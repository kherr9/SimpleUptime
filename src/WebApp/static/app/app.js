class CheckMockService {
    constructor() {
        this.checks = [{
            url: "http://example.com/"
        }, {
            url: "http://example.com/foo/"
        }];
    }
    getChecks() {
        return Promise.resolve(this.checks);
    }
}

class CheckListViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.template = '#check-template';
        this.target = '.check'
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

    updateChecks(checks) {
        var self = this;
        var model = {
            checks:checks
        };
        var html = Mustache.render(
            $(self.template).html(), 
            model);
        $(self.target).html(html);
    }
}

var checkService = new CheckMockService();
var checkListViewModel = new CheckListViewModel(checkService)

$(function () {
    checkListViewModel.init();
});