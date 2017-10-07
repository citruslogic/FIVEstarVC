(function () {
    'use strict';

    angular
        .module('fivestarApp')
        .controller('residentController', residentController);

    controller.$inject = ['$scope'];

    function controller($scope) {
        $scope.title = 'ResidentController';

        activate();

        function activate() { }
    }
})();
