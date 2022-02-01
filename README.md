# ZomboidBot

# Angular

## ClientApp/angular.json:
```jsoniq
{
  "projects": {
    "<SOLUTION_NAME>": {
      ...
      "architect": {
        "build": {
          ...
          "options": {
            ...
            "index": "src/index.html",
            "main": "src/main.ts",
            "polyfills": "src/polyfills.ts",
            "tsConfig": "src/tsconfig.app.json"
          }
        },
        "serve": {
          ...
          "configurations": {
            ...
            "development": {
              ...
              "proxyConfig": "proxy.conf.js"
            }
        }
    }
}
```
## ClientApp/package.json
```jsoniq
{
  ...
  "scripts": {
    ...
    "start": "ng serve --port 44407",
    "watch": "ng build --watch --configuration development"
  }
}
```
