app.config([
    "$stateProvider",
    function($stateProvider) {
        "use strict";

        $stateProvider
            .state("accelerator",
            {
                url: "/Litium/appSettings/accelerator"
            })
            .state("accelerator.indexing", {
                url: "/indexing",
                views: {
                    'content@': {
                        templateUrl: "/Site/Administration/Settings/Indexing.html",
                        controller: "acceleratorAdministrationIndexingController"
                    }
                }
            })
            .state("accelerator.filter", {
                url: "/filter",
                views: {
                    'content@': {
                        templateUrl: "/Site/Administration/Settings/Filter.html",
                        controller: "acceleratorAdministrationFilterController"
                    }
                }
            })
            .state("accelerator.searchindexing", {
                url: "/searchindexing",
                views: {
                    'content@': {
                        templateUrl: "/Site/Administration/Settings/SearchIndexing.html",
                        controller: "acceleratorAdministrationSearchIndexingController"
                    }
                }
            });
    }
]);