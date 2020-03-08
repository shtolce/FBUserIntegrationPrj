(function () {
  'use strict';
  angular.module('siemens.simaticit.common')
    .constant('CONFIG', {
  "type": "rt",
  "title": "Test UI Application",
  "logLevel": "Verbose",
  "theme": "Light",
  "logo": "common/images/SiemensLogo.png",
  "languages": [
    "en-US"
  ],
  "home": "home.IntegrationApp_FBUserIntegrationApp_UserIntegrationModule_UserIntegrationScreen",
  "masterApp": "IntegrationApp.FBUserIntegrationApp",
  "dependencies": [
    "IntegrationApp.FBUserIntegrationApp"
  ],
  "menu": [
    {
      "id": "home.IntegrationApp_FBUserIntegrationApp_UserIntegrationModule_UserIntegrationScreen",
      "title": "UserIntegrationScreen",
      "icon": "fa-folder",
      "display": true,
      "securable": false
    }
  ],
  "clientID": "123",
  "runtimeServicesUrl": "http://localhost/sit-svc/runtime/odata/",
  "identityProviderUrl": "http://localhost/IPSimatic-Logon",
  "authorizationServiceUrl": "http://localhost/sit-auth/OAuth/Authorize",
  "engineeringServicesUrl": "http://localhost/sit-svc/engineering/odata/",
  "applicationServiceUrls": {
    "FBUserIntegrationApp": "http://localhost/sit-svc/Application/FBUserIntegrationApp/odata"
  },
  "applicationArchivingServiceUrls": {
    "FBUserIntegrationApp": "http://localhost/sit-arch/Application/FBUserIntegrationApp/odata"
  },
  "applicationSignalManagerUrls": {
    "FBUserIntegrationApp": "http://localhost/sit-svc/Application/FBUserIntegrationApp/signals"
  }
});
})();