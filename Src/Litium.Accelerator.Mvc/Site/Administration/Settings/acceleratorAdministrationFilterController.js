app.controller("acceleratorAdministrationFilterController", [
    "$scope", "$stateParams", "languagesService", "$q", "$resource",
    function($scope, $stateParams, languagesService, $q, $resource) {
        "use strict";

        var service = $resource("/site/administration/api/filtering/setting");

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
            service.create($scope.model).$promise.then(function(response) {
                $scope.modelServer = angular.copy(response);
            }).catch(function(error) {
                $scope.modelState = error.data.modelState;
            })
        };

        $scope.add = function (field) {
            if (!Array.isArray($scope.model.items))
                $scope.model.items = [];

            if (field === undefined || field == null) return;

            $scope.model.items.push(field);
            $scope.model.filters.splice($scope.model.filters.indexOf(field), 1);
        };

        $scope.remove = function (field) {
            $scope.model.filters.push(field);
            $scope.model.items.splice($scope.model.items.indexOf(field), 1);
        };
    }
]);