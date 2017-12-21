import $ from 'jquery-slim';

export default class AddCheckViewModel {
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