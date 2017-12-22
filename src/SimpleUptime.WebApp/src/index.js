import $ from 'jquery-slim';
import Navigo from 'navigo'; 
import CheckService from './CheckService';
import AddCheckViewModel from './AddCheckViewModel';
import CheckListViewModel from './CheckListViewModel';

var checkService = null;

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
      viewModel = new CheckListViewModel(checkService, 90000);
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
})($);