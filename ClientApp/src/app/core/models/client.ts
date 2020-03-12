export class Client {
    id: string;
    clientTypeId: number;
    enabled: boolean;
    clientId: string;
    clientName: string;

    description: string;
    clientUri: string;
    logoUri: string;
    requireConsent: string;

    alwaysIncludeUserClaimsInIdToken: boolean;
    allowAccessTokensViaBrowser: boolean;

    requireClientSecret: boolean;
    identityTokenLifetime: number;
    redirectUris: string[];
    allowedCorsOrigins: string[];
    allowedScopes: string[];

    clientSecrets: any[]
    allowedGrantTypes: string[]
}
