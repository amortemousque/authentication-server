import { getHost } from "../app/core/utils/global.utils";

const baseUri = getHost();
const baseUriNoPort = getHost(false);

export const environment = {
  production: true,
  baseUri: baseUri,
  adminApiUrl: `${baseUriNoPort}:5001/v1`,
  blobApiUrl: 'https://lgroupeservices.blob.core.windows.net/lnh/',
  authConfig: {
    clientId: '60D0SPSVOBHAJOLGLHK1I8UUCO5MM249',
    issuer: `${baseUriNoPort}:5001`,
    audience: 'quarksupone_api',
    redirectUri: `${baseUri}/callback`,
    scope: `openid email profile role permission tenant quarksupone_api`,
    loginUrl:  `${baseUri}/Account/Login`,
  }
};
