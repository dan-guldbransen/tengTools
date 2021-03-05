app.controller("acceleratorAdministrationSearchIndexingController", [
    "$scope", "$stateParams", "languagesService", "$q", "$resource", '$filter',
    function ($scope, $stateParams, languagesService, $q, $resource, $filter) {
        "use strict";

        var service = $resource("/site/administration/api/searchindexing");

        $scope.dataHasChanged = false;

        service.get().$promise.then(function (response) {
            $scope.model = response;
            $scope.modelServer = angular.copy(response);
        }, angular.noop);

        $scope.$watch("model", function (newValue, oldValue) {
            if (newValue !== oldValue) {
                $scope.dataHasChanged = !angular.equals($scope.model, $scope.modelServer);
            }
        }, true);

        $scope.saveForm = function (form) {
            service.update($scope.model).$promise.then(function (response) {
                $scope.modelServer = angular.copy(response);
            }).catch(function (error) {
                $scope.modelState = error.data.modelState;
            })
        };

        $scope.addField = function (template, field) {
            if (field) {
                template.selectedFields.push(field);
                var index = template.fields.indexOf(field);
                template.fields.splice(index, 1);
            }
        };

        $scope.removeField = function (template, field, index) {
            template.fields.push(field);
            template.fields = $filter('orderBy')(template.fields, 'title');
            template.selectedFields.splice(index, 1);
        };
        $scope.removeAllFields = function (template) {
            for (var i = 0; i < template.selectedFields.length; i++) {
                template.fields.push(template.selectedFields[i]);
            }
            template.fields = $filter('orderBy')(template.fields, 'title');
            template.selectedFields = [];
        };
    }
]);