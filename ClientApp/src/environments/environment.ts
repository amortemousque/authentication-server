import { getHost } from "../app/core/utils/global.utils";

const baseUri = getHost();
const baseUriNoPort = getHost(false);

export const environment = {
  production: false,
  baseUri: baseUri,
  adminApiUrl: `${baseUriNoPort}:5001/v1`,
  blobApiUrl: 'https://lgroupeservices.blob.core.windows.net/lnh/',
  authConfig: {
    clientId: '60D0SPSVOBHAJOLGLHK1I8UUCO5MM249',
    issuer: `${baseUriNoPort}:5001`,
    audience: 'quarksupone_api',
    redirectUri: `${baseUri}`,
    scope: `openid email profile role permission tenant quarksupone_api`,
    loginUrl:  `${baseUri}/Account/Login`,
  }
};

/*
 * In development mode, to ignore zone related error stack frames such as
 * `zone.run`, `zoneDelegate.invokeTask` for easier debugging, you can
 * import the following file, but please comment it out in production mode
 * because it will have performance impact when throw error
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
