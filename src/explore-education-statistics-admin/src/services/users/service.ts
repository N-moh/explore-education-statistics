import client from '@admin/services/util/service';
import { UserStatus, UserInvite } from '@admin/services/users/types';

export interface UsersService {
  getUsers(): Promise<UserStatus[]>;
  getPendingInvites(): Promise<string[]>;
  inviteUser: (invite: UserInvite) => Promise<boolean>;
}

const service: UsersService = {
  getUsers(): Promise<UserStatus[]> {
    return client.get<UserStatus[]>('/bau/users');
  },
  getPendingInvites(): Promise<string[]> {
    return client.get<string[]>('/bau/users/pending');
  },
  inviteUser(invite: UserInvite): Promise<boolean> {
    return client.post(`/bau/users/invite`, invite);
  },
};

export default service;