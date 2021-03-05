app.controller("acceleratorAdministrationIndexingController", [
    "$scope", "$stateParams", "languagesService", "$q", "$resource",
    function($scope, $stateParams, languagesService, $q, $resource) {
        "use strict";

        var service = $resource("/site/administration/api/indexing");

        $scope.dataHasChanged = false;

        service.get().$promise.then(function(response) {
            $scope.model = response;
            $scope.modelServer = angular.copy(response);
        }, angular.noop);

        $scope.$watch("model", function(newValue, oldValue) {
            if (newValue !== oldValue) {
                $scope.dataHasChanged = !angular.equals($scope.model, $scope.modelServer);
            }
        }, true);

        $scope.saveForm = function(form) {
            service.update($scope.model).$promise.then(function(response) {
                $scope.modelServer = angular.copy(response);
            }).catch(function(error) {
                $scope.modelState = error.data.modelState;
            })
        };
    }
]);