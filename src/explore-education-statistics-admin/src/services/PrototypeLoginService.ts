import { User } from '@admin/services/sign-in/types';

export default class PrototypeLoginService {
  private static USERS: User[] = [
    {
      id: '4add7621-4aef-4abc-b2e6-0938b37fe5b9',
      name: 'John Smith',
      permissions: {
        canAccessSystem: false,
        canAccessPrereleasePages: false,
        canAccessAnalystPages: false,
        canAccessUserAdministrationPages: false,
        canAccessMethodologyAdministrationPages: false,
      },
    },
    {
      id: '8e3a250b-6153-4c5e-aba5-363a554bc288',
      name: 'Ann Evans',
      permissions: {
        canAccessSystem: false,
        canAccessPrereleasePages: false,
        canAccessAnalystPages: false,
        canAccessUserAdministrationPages: false,
        canAccessMethodologyAdministrationPages: false,
      },
    },
    {
      id: 'b7630cce-7f5f-4233-90fe-a8c751b1c38c',
      name: 'Stephen Doherty',
      permissions: {
        canAccessSystem: false,
        canAccessPrereleasePages: false,
        canAccessAnalystPages: false,
        canAccessUserAdministrationPages: false,
        canAccessMethodologyAdministrationPages: false,
      },
    },
    {
      id: '97d839e7-67fc-47b7-a40f-00f84d0cc3c4',
      name: 'BAU User',
      permissions: {
        canAccessSystem: false,
        canAccessPrereleasePages: false,
        canAccessAnalystPages: false,
        canAccessUserAdministrationPages: false,
        canAccessMethodologyAdministrationPages: false,
      },
    },
    {
      id: '6b1b5bb4-9a9b-4996-9f57-8b75176e6bf1',
      name: 'User 5',
      permissions: {
        canAccessSystem: false,
        canAccessPrereleasePages: false,
        canAccessAnalystPages: false,
        canAccessUserAdministrationPages: false,
        canAccessMethodologyAdministrationPages: false,
      },
    },
  ];

  public static getUserList(): User[] {
    return PrototypeLoginService.USERS;
  }

  public static getUser(userId: string) {
    return PrototypeLoginService.USERS.filter(user => user.id === userId)[0];
  }

  public static getAuthentication(userId: string) {
    return {
      user: PrototypeLoginService.getUser(userId),
    };
  }

  public static setActiveUser(userId: string) {
    window.localStorage.setItem('userId', userId);
  }

  public static login() {
    return PrototypeLoginService.getAuthentication(
      window.localStorage.getItem('userId') ||
        '4add7621-4aef-4abc-b2e6-0938b37fe5b9',
    );
  }
}
