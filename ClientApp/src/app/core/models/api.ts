export class Secret {
    description: string;
    value: string;
    expiration: string;
    type: string;
}

export class  ApiScope {
    id: string;
    apiResourceId: string;
    name: string;
    displayName: string;
    description: string;
    userClaims: string[];
}

export class Api {
    id: string;
    enabled: boolean;
    name: string;
    displayName: string;
    description: string;
    secrets: Secret[];
    scopes: ApiScope[];
    userClaims: string[];

}
