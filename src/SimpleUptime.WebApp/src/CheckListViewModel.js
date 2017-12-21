import $ from 'jquery-slim';
import Handlebars from 'handlebars';

export default class CheckListViewModel {
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