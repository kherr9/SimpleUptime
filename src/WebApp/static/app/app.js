class CheckMockService {
    constructor() {
        this.nextId = 1;
        this.checks = [{
            id: this.nextId++,
            url: "http://example.com/"
        }, {
            id: this.nextId++,
            url: "http://example.com/foo/"
        }];
    }

    getChecks() {
        return Promise.resolve(this.checks);
    }

    addCheck(check) {
        check.id = this.nextId++;
        this.checks.push(check);

        return Promise.resolve(check);
    }

    removeCheckById(checkId) {
        this.checks = this.checks.filter(c => c.id !== checkId);

        return Promise.resolve(true);
    }
}

class CheckService {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
    }

    getChecks() {
        return fetch(this.baseUrl + '/api/httpmonitors')
            .then(function (response) {
                return response.json();
            });
    }

    addCheck(check) {
        return fetch(this.baseUrl + '/api/httpmonitors', {
            method: 'POST',
            headers: {
                'Accept': 'application/json, text/plain, */*',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(check)
        })
            .then(function (res) {
                return JSON.stringify(res.json());
            });
    }

    removeCheckById(checkId) {
        return fetch(this.baseUrl + '/api/httpmonitors/' + checkId, {
            method: 'DELETE'
        });
    }
}

class CheckListViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.checks = [];
        this.$target = $('#main');
        this.template = Handlebars.compile($('#check-template').html())
    }

    init() {
        var self = this;

        // fetch checks, then update
        self.checkService
            .getChecks()
            .then(function (checks) {
                self.updateChecks(checks);
            });

        // set click handler for delete
        self.$target.on('click', '.delete', function (e) {
            var checkId = $(this).data('id');
            var check = self.checks.find(c => c.id == checkId);

            if (check !== undefined) {
                self.removeCheck(check)
            }
        });
    }

    dispose() {
        var self = this;

        self.$target.off();
    }

    updateChecks(checks) {
        var self = this;

        // sort checks by url
        checks.sort(function (a, b) {
            return a.request.url.localeCompare(b.request.url);
        });

        // update models with state
        for (var i = 0; i < checks.length; i++) {
            var check = checks[i];
            if (check.status === 'Up') {
                check.isUp = true;
            } else if (check.status === 'Down') {
                check.isDown = true;
            }
        }

        // save checks locally
        self.checks = checks;

        // write run tempalte and write to body
        self.$target.html(self.template({
            checks: checks
        }));
    }

    removeCheck(check) {
        var self = this;
        if (confirm('Are you sure you want to delete?')) {
            self.checkService.removeCheckById(check.id)
                .then(function (e) {
                    var modifiedChecks = self.checks.filter(c => c !== check);
                    self.updateChecks(modifiedChecks)
                });
        }
    }
}

class AddCheckViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.template = '#addcheck-template';
        this.target = 'main'
        this.$target = $('#main');
    }

    init() {
        var self = this;

        self.renderHtml();

        self.$target.on('submit', 'form', function (e) {
            self.onAddCheck(e);
        });
    }

    dispose() {
        var self = this;

        self.$target.off();
    }

    renderHtml() {
        var self = this;

        var html = $(self.template).html();

        self.$target
            .html(html)
            .find('*')
            .filter(':input:visible:first')
            .focus();
    }

    onAddCheck(e) {
        var self = this;
        e.preventDefault();

        var request = self.$target.find('form').serializeFormJSON();

        var check = {
            request: request
        };

        self.checkService.addCheck(check)
            .then(function () {
                window.location = '#/'
            })
    }
}

var checkService = null;
console.log(window.location.hostname);
if (window.location.hostname === 'localhost') {
    //var checkService = new CheckMockService();
    checkService = new CheckService('http://localhost:8010');
} else {
    checkService = new CheckService('');
}

$(function () {

    var root = null;
    var useHash = true; // Defaults to: false
    var hash = '#'; // Defaults to: '#'
    var router = new Navigo(root, useHash, hash);
    var viewModel = null;

    router.hooks({
        before: function (done, params) {
            if (viewModel !== null && typeof viewModel.dispose === 'function') {
                viewModel.dispose();
            }
            done();
        }
    })

    router
        .on(function () {
            // display all the products
            viewModel = new CheckListViewModel(checkService);
            viewModel.init();
        })
        .on('add', function () {
            viewModel = new AddCheckViewModel(checkService);
            viewModel.init();
        }).resolve();
});

(function ($) {
    $.fn.serializeFormJSON = function () {
        var o = {};
        var a = this.serializeArray();
        $.each(a, function () {
            if (o[this.name]) {
                if (!o[this.name].push) {
                    o[this.name] = [o[this.name]];
                }
                o[this.name].push(this.value || '');
            } else {
                o[this.name] = this.value || '';
            }
        });
        return o;
    };
})(jQuery);