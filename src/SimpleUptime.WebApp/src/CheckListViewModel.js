import $ from 'jquery-slim';
import checkListTemplate from './check-list.handlebars';

export default class CheckListViewModel {
    constructor(checkService, autoRefresh) {
        this.checkService = checkService;
        this.autoRefresh = autoRefresh;

        this.checks = [];
        this.$target = $('#main');
        this.intervalId = 0;
    }

    init() {
        var self = this;

        // fetch checks, then update
        self.checkAndUpdate();

        // set click handler for delete
        self.$target.on('click', '.delete', function (e) {
            var checkId = $(this).data('id');
            var check = self.checks.find(c => c.id == checkId);

            if (check !== undefined) {
                self.removeCheck(check)
            }
        });

        if (self.autoRefresh) {
            self.intervalId = window.setInterval(() => self.checkAndUpdate(), self.autoRefresh);
        }
    }

    dispose() {
        var self = this;

        self.$target.off();
        window.clearInterval(self.intervalId);
    }

    checkAndUpdate() {
        var self = this;

        self.checkService
            .getChecks()
            .then(function (checks) {
                self.updateChecks(checks);
            });
    }

    updateChecks(checks) {
        var self = this;

        // sort checks by url
        checks.sort(function (a, b) {
            return a.request.url.localeCompare(b.request.url);
        });

        // modify checks and save locally
        self.checks = checks.map(x => {
            // set last check info
            if (x.recentHttpMonitorChecks !== null
                && x.recentHttpMonitorChecks.length > 0) {

                // make the most recent check time easily accessible.
                x.lastChecked = x.recentHttpMonitorChecks[0].requestTiming.endTime;
                x.lastCheckedDisplayText = self.secondsSince(x.lastChecked);
            } else {
                x.lastChecked = null;
                x.lastCheckedDisplayText = "never checked"
            }

            // set isUp/isDown info
            if (x.status === 'Up') {
                x.isUp = true;
            } else if (x.status === 'Down') {
                x.isDown = true;
            }

            return x;
        });

        // generate html from template
        var html = checkListTemplate({
            checks: checks
        });

        // write html
        self.$target.html(html);
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
    
    secondsSince(date) {
        var date = new Date(Date.parse(date + "Z"));

        var now = new Date();
        var seconds = Math.abs(Math.floor((now - date) / 1000));

        if (seconds === 1) {
            return "1 second";
        } else {
            return seconds + " seconds";
        }
    }
}