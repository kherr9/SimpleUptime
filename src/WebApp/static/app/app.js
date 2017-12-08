class CheckMockService {
    constructor(){
        this.checks = [{
            url:"http://example.com/"
        },{
            url:"http://example.com/foo/"
        }];
    }
    getChecks() {
        return Promise.resolve(this.checks);
    }
}

class CheckListViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.selector = '#thing';
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
        console.log('update checks', checks);
    }
}

var checkService = new CheckMockService();
var checkListViewModel = new CheckListViewModel(checkService)

$(function () {
    checkListViewModel.init();
});