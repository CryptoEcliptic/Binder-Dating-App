--CoreKraft provides the clients functionality to override the system plugins that are registered in the appsettings.json.
--For that purpose there is a OverrideModuleSetting class which contains data loaders containing the custom settings.
//You don't need to override settings in that case will take defaults
//Service and component modules: These can be used in many Apps, and are common for all running entities in the BindKraft session. 

# Changing the default behaviour of a particular loader or a collection of loaders for a module.

## The default behaviour:
 - The dependancies of the module are read from Dependancy.json file. (Example of Dependancy.json file bellow)
```json
{
  "name": "Board",
  "version": "1.0.0",
  "description": "Description of the project",
  "keywords": [],
  "author": "Clean Code Factory",
  "license": "MIT License",
  "dependencies": {
    "PlatformUtility": "^1.0.0"
    ...
  },
  "signals": [
    ...
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
              ...
            },
            {
              "Name": "SqLite",
              ...
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
## The overriding process:
In order to override the default settings for each module you should add a section (OverrideModuleSettings) in the corresponding appsettings.json file.
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
     
    OverrideModuleSettings: 
    [
      {
        "ModuleName": "Board",
        "Loaders": [
          {
            "LoaderName": "SqLite",
            "CustomSettings": {
              "ConnectionString": "Data Source=@connectionStringData",
              "NoCache": false
            }
          }
        ]
      }
    ]
  }
}
```



