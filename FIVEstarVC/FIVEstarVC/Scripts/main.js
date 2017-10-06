"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var platform_browser_dynamic_1 = require("@angular/platform-browser-dynamic");
var boot_1 = require("./boot");
var platform = platform_browser_dynamic_1.platformBrowserDynamic();
platform.bootstrapModule(boot_1.AppModule);
