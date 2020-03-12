
export class AuthUser {
    id: string;
    tenantId: string;
    tenantName: string;
    name: string;
    givenName: string;
    familyName: string;
    email: string;
    address: string;
    role: any[];
    permission: any[];
    // todo get talentId
    talentId: string;
    constructor() {
    }

    mapClaims(claims) {
        this.id = claims.sub;
        this.name = claims.name;
        this.familyName = claims.family_name;
        this.email = claims.email;
        this.givenName = claims.given_name;
        this.tenantId = claims.tenant_id;
        this.tenantName = claims.tenant_name;
        this.role = claims.role;
        this.permission = claims.permission;
    }
}
