/*<link rel="stylesheet" ng-href="IntegrationApp.FBUserIntegrationApp/styles/scrolled.css">*/

    (function () {
    'use strict';
    angular.module('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule').config(ListScreenRouteConfig);

    ListScreenController.$inject = ['IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen.service',
        '$state', '$stateParams', '$rootScope', '$scope', 'common.base', 'common.services.logger.service','common.widgets.busyIndicator.service',];
    function ListScreenController(dataService, $state, $stateParams, $rootScope, $scope, base, loggerService,busyIndicatorService) {
        var self = this;
        var logger, rootstate, messageservice, backendService;

        activate();

        // Initialization function
        function activate() {
            logger = loggerService.getModuleLogger('IntegrationApp.FBUserIntegrationApp.UserIntegrationModule.UserIntegrationScreen');

            init();
        }

        function init() {
            logger.logDebug('Initializing controller.......');

            rootstate = 'home.IntegrationApp_FBUserIntegrationApp_UserIntegrationModule_UserIntegrationScreen';
            messageservice = base.widgets.messageOverlay.service;
            backendService = base.services.runtime.backendService;
            
            //Initialize Model Data
            self.selectedItem = null;
            self.isButtonVisible = false;
            self.viewerOptions = {};
            self.viewerData = [];

            //Expose Model Methods
            self.importEquipmentButtonHandler = importButtonHandler;
            self.importMaterialButtonHandler = importButtonHandler;
            self.importBoMButtonHandler = importButtonHandler;
            self.addButtonHandler = addButtonHandler;
            self.editButtonHandler = editButtonHandler;
            self.selectButtonHandler = selectButtonHandler;
            self.deleteButtonHandler = deleteButtonHandler;
            self.readEquipmentButtonHandler = readEquipmentButtonHandler;
            self.readMaterialButtonHandler = readMaterialButtonHandler;
            self.readBoMButtonHandler = readBoMButtonHandler;
            self.onCheck = onCheck;
        }
        function onCheck(el){
            if((typeof el.NId !== 'undefined') & (el.BoMItem_MaterialDefinition_Name!=='undefined')) {
                self.dataArray.value.forEach((par)=>{
                    if (el.NId == par.NId & el!=par)
                        par.checked = !par.checked;
                });

            } else{
                console.log('нет поля');
            }


        }



        function importButtonHandler(clickedCommand) {
            let inputParameters = self.dataArray.value;
            busyIndicatorService.show();
            inputParameters.forEach((par)=>{
                    if (par.checked==true)    {
                        delete par.checked;
                        dataService.execCommand(this.name, par).then(function (data) {
                            console.log(data);
                        });
                    }
            });
            busyIndicatorService.hide();

        }
        function withValue(value) {
            var d = withValue.d || (
                withValue.d = {
                    enumerable: false,
                    writable: false,
                    configurable: false,
                    value: null
                }
            );
            d.value = value;
            return d;
        }
        function readEquipmentButtonHandler(clickedCommand) {

            dataService.readFunction(this.name, {}).then(function (data) {
                self.dataArray = data;
                self.dataArray.value = data.value.map( (n)=>{return {
                    checked:false,
                    Name:n.Name,
                    NId:n.NId,
                    LevelNId:n.LevelNId,
                    Description:n.Description
                }});

                self.tableKeys = Object.keys(self.dataArray.value[0]);
                //console.log(Object.keys(self.dataArray.value[0]));
            });
        }
        function readMaterialButtonHandler(clickedCommand) {

            dataService.readFunction(this.name, {}).then(function (data) {
                self.dataArray = data;
                self.dataArray.value = data.value.map( (n)=>{return {
                    checked:false,
                    Name: n.Name,
                    NId: n.NId,
                    LogisticClassNId: n.LogisticClassNId,
                    MaterialClass_Id: n.MaterialClass_Id,
                    Material_Id: n.Material_Id,
                    Revision: n.Revision,
                    UoMNId: n.UoMNId
                }});

                self.tableKeys = Object.keys(self.dataArray.value[0]);
                //console.log(Object.keys(self.dataArray.value[0]));
            });
        }

        function readBoMButtonHandler(clickedCommand) {

            dataService.readFunction(this.name, {}).then(function (data) {
                self.dataArray = data;
                self.dataArray.value = data.value.map( (n)=>{return {
                    checked:false,
                    Name:n.Name,
                    NId:n.NId,
                    Description:n.Description,
                    Quantity:n.Quantity,
                    Version:n.Version,
                    MaterialDefinition_Name:n.MaterialDefinition_Name,
                    MaterialDefinition_NId:n.MaterialDefinition_NId,
                    BoMItem_MaterialDefinition_Name:n.BoMItem_MaterialDefinition_Name,
                    BoMItem_MaterialDefinition_NId:n.BoMItem_MaterialDefinition_NId,
                    BoMItem_Name:n.BoMItem_Name,
                    BoMItem_NId:n.BoMItem_NId,
                    BoMItem_Quantity:n.BoMItem_Quantity
                }});

                self.tableKeys = Object.keys(self.dataArray.value[0]);
                //console.log(Object.keys(self.dataArray.value[0]));
            });
        }


        function addButtonHandler(clickedCommand) {
            $state.go(rootstate + '.add');
        }

        function editButtonHandler(clickedCommand) {
            // TODO: Put here the properties of the entity managed by the service
            $state.go(rootstate + '.edit', { id: self.selectedItem.Id, selectedItem: self.selectedItem });
        }

        function selectButtonHandler(clickedCommand) {
            // TODO: Put here the properties of the entity managed by the service
            $state.go(rootstate + '.select', { id: self.selectedItem.Id, selectedItem: self.selectedItem });
        }

        function deleteButtonHandler(clickedCommand) {
            var title = "Delete";
            // TODO: Put here the properties of the entity managed by the service
            var text = "Do you want to delete '" + self.selectedItem.Id + "'?";

            backendService.confirm(text, function () {
                dataService.delete(self.selectedItem).then(function () {
                    $state.go(rootstate, {}, { reload: true });
                }, backendService.backendError);
            }, title);
        }

        function onGridItemSelectionChanged(items, item) {
            if (item && item.selected == true) {
                self.selectedItem = item;
                setButtonsVisibility(true);
            } else {
                self.selectedItem = null;
                setButtonsVisibility(false);
            }
        }

        // Internal function to make item-specific buttons visible
        function setButtonsVisibility(visible) {
            self.isButtonVisible = visible;
        }
    }

    ListScreenRouteConfig.$inject = ['$stateProvider'];
    function ListScreenRouteConfig($stateProvider) {
        var moduleStateName = 'home.IntegrationApp_FBUserIntegrationApp_UserIntegrationModule';
        var moduleStateUrl = 'IntegrationApp_FBUserIntegrationApp_UserIntegrationModule';
        var moduleFolder = 'IntegrationApp.FBUserIntegrationApp/modules/UserIntegrationModule';

        var state = {
            name: moduleStateName + '_UserIntegrationScreen',
            url: '/' + moduleStateUrl + '_UserIntegrationScreen',
            views: {
                'Canvas@': {
                    templateUrl: moduleFolder + '/UserIntegrationScreen-list.html',
                    controller: ListScreenController,
                    controllerAs: 'vm'
                }
            },
            data: {
                title: 'UserIntegrationScreen'
            }
        };
        $stateProvider.state(state);
    }
}());
