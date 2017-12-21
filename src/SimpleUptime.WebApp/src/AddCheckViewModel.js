import $ from 'jquery-slim';
import checkAddTemplate from './check-add.handlebars';

export default class AddCheckViewModel {
    constructor(checkService) {
        this.checkService = checkService;
        this.$target = $('body');
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

        var html = checkAddTemplate({});

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