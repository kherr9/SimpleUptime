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
}

class CheckListViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.template = '#check-template';
        this.target = 'main'
    }

    init() {
        var self = this;

        self.checkService
            .getChecks()
            .then(function (checks) {
                console.log('got checks');
                self.updateChecks(checks);
            });
    }

    dispose() {
        $(this.target).off();
    }

    updateChecks(checks) {
        var self = this;
        var model = {
            checks: checks
        };
        var html = Mustache.render(
            $(self.template).html(),
            model);
        $(self.target).html(html);
    }
}

class AddCheckViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.template = '#addcheck-template';
        this.target = 'main'

        console.log('check server', this.checkService);
    }

    init() {
        self = this;
        self.renderHtml();

        $(this.target).on('submit', 'form', function (e) {
            self.onAddCheck(e);
        });
    }

    dispose() {
        $(this.target).off();
    }

    renderHtml() {
        var self = this;

        var html = $(self.template).html();

        $(self.target).html(html);
    }

    onAddCheck(e) {
        var self = this;
        e.preventDefault();

        var check = $(self.target).find('form').serializeFormJSON();

        self.checkService.addCheck(check)
            .then(function () {
                window.location = '#/'
            })
    }
}

var checkService = new CheckMockService();

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