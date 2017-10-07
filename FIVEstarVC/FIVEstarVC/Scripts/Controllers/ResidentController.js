(function () {
    'use strict';

    angular
        .module('fivestarApp')
        .controller('ResidentController', ResidentController);


    $.scope.arrivaldate = new Date();
    $.scope.departdate = new Date();

    controller.$inject = ['$scope'];

    function controller($scope) {
        $scope.title = 'ResidentController';

        activate();

        function activate() { }
    }


})();