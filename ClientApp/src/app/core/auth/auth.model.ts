export interface Space {
    id?: number;
    label?: string;
}

export interface AuthUser {
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
}


