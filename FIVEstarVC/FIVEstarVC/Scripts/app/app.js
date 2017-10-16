var app = angular.module('FiveStarApp', ['ui.bootstrap']);
app.controller('ResidentController', ['$scope', '$uibModal',

    function ($scope, $uibModal) {

        // $scope.answer = function () {
        //     return $scope.correctAnswer ? 'correct' : 'incorrect';
        // };
        .when('/addresident', {
            templateUrl: '/Home',
            controller: 'ngOrderController' // Angular Controller
        })

        $scope.residentAdd = function () {
            $scope.message = "Show Form Button Clicked";

            var modalInstance = $uibModal.open({
                templateUrl: 'AddResident.cshtml',
                controller: ModalInstanceCtrl,
                scope: $scope,
                resolve: {
                    userForm: function () {
                        return $scope.userForm;
                    }
                }
            });



        };

    }]);

var ModalInstanceCtrl = function ($scope, $modalInstance, userForm) {
    $scope.form = {}
    $scope.submitForm = function () {
        if ($scope.form.userForm.$valid) {
            $modalInstance.close('closed');

        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel');
        };



    };
};