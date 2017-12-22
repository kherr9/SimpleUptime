export default class CheckService {
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