export const driveApi = {
  user: {
    verify: 'api/User/verify',
    login: 'api/User/token',
    list: 'api/User',
    get: 'api/User/{userId}',
    create: 'api/User',
    update: 'api/User',
    delete: 'api/User/{userId}'
  },
  file: {
    list: 'api/File/{path}',
    upload: 'api/File/{path}',
    createChild: 'api/File/createChild/{path}',
    move: 'api/File',
    delete: 'api/File/delete'
  }
};
