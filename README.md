--CoreKraft provides the clients functionality to override the system plugins that are registered in the appsettings.json.
--For that purpose there is a OverrideModuleSetting class which contains data loaders containing the custom settings.
//You don't need to override settings in that case will take defaults
//Service and component modules: These can be used in many Apps, and are common for all running entities in the BindKraft session. 

Title: Changing the default behaviour of a particular loader or a collection of loaders for a module.

The default behaviour:
 - The dependancies of the module are read from Dependancy.json file. (Example of Dependancy.json file bellow)
```json
{
  "name": "Board",
  "version": "1.0.0",
  "description": "The effective application for keeping track of tasks in project. Organize multiple projects in one place, create tasks and make assignments, attached files or share boards",
  "keywords": [],
  "author": "Clean Code Factory",
  "license": "MIT License",
  "dependencies": {
    "BindKraftGenericControls": "^1.0.0",
    "PlatformUtility": "^1.0.0",
    "BindKraftStyles": "^1.0.0",
    "KraftNotify": "^1.0.0",
    "BindKraftUI": "^1.0.0",
    "UserProfile": "^1.0.0"
  },
  "signals": [
    {
      "key": "DeleteUser",
      "nodeset": "deleteuser",
      "nodepath": "tenant",
      "maintenance": false
    },
    {
      "key": "OnSystemStartUp",
      "nodeset": "boardtenant",
      "nodepath": "checktenant",
      "maintenance": true
    }
  ]
}
```
 - Configuration settings of the module are read from the Configuration.json file. (Example of Configuration.json file bellow)
```json
{
  "KraftModuleConfigurationSettings": {
    "NodeSetSettings": {
      "SourceLoaderMapping": {
        "NodesDataIterator": {
          "NodesDataIteratorConf": {
            "Name": "DefaultNodeTreeIterator",
            "ImplementationAsString": "Ccf.Ck.SysPlugins.Iterators.DataNodes.DataNodesImp, Ccf.Ck.SysPlugins.Iterators.DataNodes",
            "InterfaceAsString": "Ccf.Ck.SysPlugins.Interfaces.IDataIteratorPlugin, Ccf.Ck.SysPlugins.Interfaces",
            "Default": true,
            "CustomSettings": {
              "MyCustomsetting": "Iterator"
            }
          },
          "NodesDataLoader": [
            {
              "Name": "JsonData",
              "ImplementationAsString": "Ccf.Ck.SysPlugins.Data.Json.JsonDataImp, Ccf.Ck.SysPlugins.Data.Json",
              "InterfaceAsString": "Ccf.Ck.SysPlugins.Interfaces.IDataLoaderPlugin, Ccf.Ck.SysPlugins.Interfaces",
              "Default": true,
              "CustomSettings": {
                "BasePath": "@moduleroot@/Data/"
              }
            },
            {
              "Name": "SqLite",
              "ImplementationAsString": "Ccf.Ck.SysPlugins.Data.Db.ADO.GenericSQLite, Ccf.Ck.SysPlugins.Data.Db.ADO",
              "InterfaceAsString": "Ccf.Ck.SysPlugins.Interfaces.IDataLoaderPlugin, Ccf.Ck.SysPlugins.Interfaces",
              "Default": true,
              "CustomSettings": {
                "ConnectionString": "Data Source=@moduleroot@/Data/Activity%tenantid%.sqlite;",
                "NoCache": false
              }
            }
          ]
        },
        "LookupLoader": [
        ],
        "ResourceLoader": [
        ]
      }
    }
  }
}
```
 - The application settings are read from the appsettings.json file. Here the default values for each module are load. (Example of appsettings.json file bellow)
```json
{
  "KraftGlobalConfigurationSettings": {
    "GeneralSettings": {
      "EnableOptimization": false,
      "ModulesRootFolders": [
        "@contentroot@/Modules/",
        "@contentroot@/bin/Debug/netcoreapp2.2/Modules/"
      ],
      "DefaultStartModule": "BindKraftIntro",
      "Theme": "Basic",
      "KraftUrlSegment": "node",
      "KraftUrlCssJsSegment": "res",
      "KraftUrlResourceSegment": "raw",
      "KraftUrlModuleImages": "images",
      "KraftUrlModulePublic": "public",
      "KraftRequestFlagsKey": "sysrequestcontent",
    
      "HostingServiceSettings": [
        {
          "IntervalInMinutes": 0,
          "Signals": [
            "UpdateTenant"
          ]
        }
      ],
      "SignalSettings": {
        "OnSystemStartup": [
          "OnSystemStartup"
        ],
        "OnSystemShutdown": []
      },
      "SignalRSettings": {
        "UseSignalR": false,
        "HubImplementationAsString": "",
        "HubRoute": "/hub"
      }
    }
  }
}
```
The overriding process:
In order to override the default settings for each module you should add a section ("OverrideModuleSettings") in the corresponding appsettings.json file.
In the section you should provide an array of objects. Each object contains ModuleName, Collection of Loaders where you should provide LoaderName and 
CustomSettings object that will override the default behaviour of the module's loader. 
(See the example bellow)
```json
{
  "KraftGlobalConfigurationSettings": {
    "GeneralSettings": {
      "EnableOptimization": false,
      "ModulesRootFolders": [
        "@contentroot@/Modules/"
      ],
      "DefaultStartModule": "KraftApps_Launcher",
      "Theme": "Basic",
      "KraftUrlSegment": "node",
      "KraftUrlCssJsSegment": "res",
      "KraftUrlResourceSegment": "raw",
      "KraftUrlModuleImages": "images",
      "KraftUrlModulePublic": "public",
      "KraftRequestFlagsKey": "sysrequestcontent",
     
    "OverrideModuleSettings": 
    [
      {
        "ModuleName": "Board",
        "Loaders": [
          {
            "LoaderName": "SqLite",
            "CustomSettings": {
              "ConnectionString": "Data Source=@moduleroot@/Data/Activity%tenantid%.sqlite;",
              "NoCache": false
            }
          }
        ]
      }
    ]
  }
}
```



