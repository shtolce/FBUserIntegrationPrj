﻿(function () {
    'use strict';
    angular.module('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule')
        .constant('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen.constants', ScreenServiceConstants())
        .service('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen.service', ScreenService)
        .run(ScreenServiceRun);

    function ScreenServiceConstants() {
        return {
            data: {
                //appName: 'FBUserIntegrationApp',
                appName: 'FBUserIntegrationApp', // The name of App: in case of extended app, it is the name of AppBase
                appPrefix: 'IntegrationApp',
                // TODO: Customize the entityName with the name of the entity defined in the App you want to manage within the UI Module.
                //       Customize the command name with the name of the command defined in the App you want to manage within the UI Module
                entityName: null,
                createPublicName: null,
                updatePublicName: null,
                deletePublicName: null
            }
        };
    }

    ScreenService.$inject = ['$q', '$state', 'common.base', 'IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen.constants', 'common.services.logger.service'];
    function ScreenService($q, $state, base, context, loggerService) {
        var self = this;
        var logger, backendService;

        activate();
        function activate() {
            logger = loggerService.getModuleLogger('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen.service');
            backendService = base.services.runtime.backendService;
            exposeApi();
        }

        function exposeApi() {
            self.getAll = getAll;
            self.create = createEntity;
            self.update = updateEntity;
            self.delete = deleteEntity;
            self.execCommand = execCommand;
            self.readFunction = readFunction;
        }

        function getAll(options) {
            return execGetAll(options);
        }

        function createEntity(data) {
            // TODO: Customize the mapping between "UI entity" and the "DB entity" that will create 
            var obj = {
                'Id': data.Id
            };
            return execCommand(context.data.createPublicName, obj);
        }

        function updateEntity(data) {
            // TODO: Customize the mapping between "UI entity" and the "DB entity" that will create
            var obj = {
                'Id': data.Id
            };
            return execCommand(context.data.updatePublicName, obj);
        }

        function deleteEntity(data) {
            // TODO: Customize the mapping between "UI entity" and the "DB entity" that will delete
            var obj = {
                'Id': data.Id
            };
            return execCommand(context.data.deletePublicName, obj);
        }

        function execGetAll(options) {
            return backendService.findAll({
                'appName': context.data.appName,
                'entityName': context.data.entityName,
                'options': options
            });
        }

        function execCommand(publicName, params) {
            logger.logDebug('Executing command.......', publicName);
            return backendService.invoke({
                'appName': context.data.appName,
                'commandName': publicName,
                'params': params
            });
        }


        function readFunction(publicName, params) {
            logger.logDebug('Executing command.......', publicName);
            return backendService.read({
                'appName': context.data.appName,
                'functionName': publicName,
                'params': params
            });
        }




    }

    ScreenServiceRun.$inject = ['IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen.constants', 'common.base'];
    function ScreenServiceRun(context, common) {
        if (!context.data.entityName) {
            common.services.logger.service.logWarning('Configure the entityName');
        };
        if (!context.data.createPublicName) {
            common.services.logger.service.logWarning('Configure the createPublicName');
        };
        if (!context.data.deletePublicName) {
            common.services.logger.service.logWarning('Configure the deletePublicName');
        };
        if (!context.data.updatePublicName) {
            common.services.logger.service.logWarning('Configure the updatePublicName');
        };
    }
}());
