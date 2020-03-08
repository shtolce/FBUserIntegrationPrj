(function(){
    'use strict';

    angular.module('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule', []).config(StateConfig);

    StateConfig.$inject = ['$stateProvider'];
    function StateConfig($stateProvider) {
        var moduleStateName = 'home.IntegrationApp_FBUserIntegrationApp_UserIntegrationModule';
        var moduleStateUrl = 'IntegrationApp_FBUserIntegrationApp_UserIntegrationModule';
        var moduleFolder = 'IntegrationApp.FBUserIntegrationApp/modules/UserIntegrationModule';

        //Add new states under the root state to be unique. Below is an example code for reference
        //var state1 = {
        //    name: moduleStateName + '_state_name',
        //    url: '/' + moduleStateUrl + '_state_url',
        //    views: {
        //        'Canvas@': {
        //            templateUrl: moduleFolder + '/state_template.html',
        //            controller: 'state_controller',
        //            controllerAs: 'vm'
        //        }
        //    },
        //    data: {
        //        title: 'state_title'
        //    },
        //    params: {}
        //};
        //$stateProvider.state(state1);
    }
}());
