// This file can be replaced during build by using the `fileReplacements` array.
// `ng build ---prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  config: {
    tenant: 'verawaterandpower.com',
    clientId: '4e2ad391-77f4-4dd9-bd2b-58125f55e5ba',
    endpoints: {
      'https://bigfoot.verawaterandpower.com/': 'a809a684-750b-4a1e-a0fb-ee3debe27313 ',
      //'https://bigfoot.verawaterandpower.com/api/Login': 'a809a684-750b-4a1e-a0fb-ee3debe27313 ',
      //'https://bigfoot.verawaterandpower.com/api/TravelAuth': 'a809a684-750b-4a1e-a0fb-ee3debe27313 ',

    }
  }
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
