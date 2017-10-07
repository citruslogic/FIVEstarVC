angular.module('FiveStarApp', [])
    .controller('ResidentController', function ($scope, $http) {
        $scope.arrivaldate = new Date();
        $scope.departdate = new Date();
        $scope.title = "Center Residents";


       // $scope.answer = function () {
       //     return $scope.correctAnswer ? 'correct' : 'incorrect';
       // };
    });