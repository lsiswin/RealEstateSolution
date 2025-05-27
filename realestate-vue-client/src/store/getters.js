const getters = {
  // 用户相关
  token: state => state.user.token,
  userId: state => state.user.userId,
  username: state => state.user.username,
  realName: state => state.user.realName,
  avatar: state => state.user.avatar,
  roles: state => state.user.roles,
  permissions: state => state.user.permissions,
  
  // 权限相关
  routes: state => state.permission.routes,
  addRoutes: state => state.permission.addRoutes,
  
  // 应用相关
  sidebar: state => state.app.sidebar,
  size: state => state.app.size,
  device: state => state.app.device,
  
  // 标签页
  visitedViews: state => state.tagsView.visitedViews,
  cachedViews: state => state.tagsView.cachedViews
}

export default getters 