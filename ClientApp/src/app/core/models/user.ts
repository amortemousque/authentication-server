export class UserLogin {
    loginProvider: string;
    providerDisplayName: string;
    providerKey: string;
}

export class UserClaim {
    type: string;
    value: string;
}

export class User {
    id: string;
    tenantId: string;
    givenName: string;
    familyName: string;
    fullName: string;
    lockoutEndDateUtc: string
    normalizedFullName: string;
    email: string;
    phoneNumber: number;
    accessFailedCount: number;
    logins: UserLogin[];
    claims: UserClaim[];
    roles: string[];
    registrationDate: string;
    lastLoginDate: string;
    emailConfirmed: boolean;
    logoUri: string;

}
