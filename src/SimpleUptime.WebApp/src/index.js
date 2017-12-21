import _ from 'lodash';
import $ from 'jquery-slim';
import Navigo from 'navigo'; 
import printMe from './print.js';
//import './styles.css';
import { cube } from './math.js';
import CheckService from './CheckService';
import AddCheckViewModel from './AddCheckViewModel';
import CheckListViewModel from './CheckListViewModel';

/*
if(process.env.NODE_ENV !== 'production'){
  console.log('Looks like we are in development mode!')
}
*/

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
})($);

/*
function component() {
  var element = document.createElement('div');
  var btn = document.createElement('button');

  element.innerHTML = _.join(['Hello', 'webpack', cube(5)], ' ');

  btn.innerHTML = 'Click me and check the console';
  btn.onclick = printMe;

  element.appendChild(btn);

  return element;
}

let element = component(); // Store the element to re-render on print.js changes
document.body.appendChild(element);

if (module.hot) {
  module.hot.accept('./print.js', function () {
    console.log('Accepting the updated printMe module!');
    printMe();
    document.body.removeChild(element);
    element = component(); // Re-render the "component" to update the click handler
    document.body.appendChild(element);
  })
}
*/