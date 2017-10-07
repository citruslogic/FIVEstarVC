var app = angular.module('FiveStarApp', ['ui.bootstrap']);
    app.controller('ResidentController', ['$scope', '$modal', 
       
        function ($scope, $modal) {

       // $scope.answer = function () {
       //     return $scope.correctAnswer ? 'correct' : 'incorrect';
       // };
   

            $scope.showForm = function () {
                $scope.message = "Show Form Button Clicked";

                var modalInstance = $modal.open({
                    templateUrl: 'modal-form.html',
                    controller: ModalInstanceCtrl,
                    scope: $scope,
                    resolve: {
                        userForm: function () {
                            return $scope.userForm;
                        }
                    }
                });


            };

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
  