(window["webpackJsonp"] = window["webpackJsonp"] || []).push([["polyfills"],{

/***/ "../../node_modules/@angular-devkit/build-angular/src/angular-cli-files/models/jit-polyfills.js":
/*!*************************************************************************************************************************!*\
  !*** C:/Projekt/TengTools/Src/node_modules/@angular-devkit/build-angular/src/angular-cli-files/models/jit-polyfills.js ***!
  \*************************************************************************************************************************/
/*! no exports provided */
/***/ (function(module, __webpack_exports__, __webpack_require__) {

"use strict";
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var core_js_proposals_reflect_metadata__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! core-js/proposals/reflect-metadata */ "../../node_modules/core-js/proposals/reflect-metadata.js");
/* harmony import */ var core_js_proposals_reflect_metadata__WEBPACK_IMPORTED_MODULE_0___default = /*#__PURE__*/__webpack_require__.n(core_js_proposals_reflect_metadata__WEBPACK_IMPORTED_MODULE_0__);
/**
 * @license
 * Copyright Google Inc. All Rights Reserved.
 *
 * Use of this source code is governed by an MIT-style license that can be
 * found in the LICENSE file at https://angular.io/license
 */



/***/ }),

/***/ "../../node_modules/core-js/internals/bind-context.js":
/*!*******************************************************************************!*\
  !*** C:/Projekt/TengTools/Src/node_modules/core-js/internals/bind-context.js ***!
  \*******************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

var aFunction = __webpack_require__(/*! ../internals/a-function */ "../../node_modules/core-js/internals/a-function.js");

// optional / simple context binding
module.exports = function (fn, that, length) {
  aFunction(fn);
  if (that === undefined) return fn;
  switch (length) {
    case 0: return function () {
      return fn.call(that);
    };
    case 1: return function (a) {
      return fn.call(that, a);
    };
    case 2: return function (a, b) {
      return fn.call(that, a, b);
    };
    case 3: return function (a, b, c) {
      return fn.call(that, a, b, c);
    };
  }
  return function (/* ...args */) {
    return fn.apply(that, arguments);
  };
};


/***/ }),

/***/ 2:
/*!*******************************************************************************************************************************!*\
  !*** multi C:/Projekt/TengTools/Src/node_modules/@angular-devkit/build-angular/src/angular-cli-files/models/jit-polyfills.js ***!
  \*******************************************************************************************************************************/
/*! no static exports found */
/***/ (function(module, exports, __webpack_require__) {

module.exports = __webpack_require__(/*! C:\Projekt\TengTools\Src\node_modules\@angular-devkit\build-angular\src\angular-cli-files\models\jit-polyfills.js */"../../node_modules/@angular-devkit/build-angular/src/angular-cli-files/models/jit-polyfills.js");


/***/ })

},[[2,"manifest","vendor"]]]);
//# sourceMappingURL=polyfills.js.map