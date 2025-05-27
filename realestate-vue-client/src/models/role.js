/**
 * 角色模型 - 对应RoleController的RoleDto
 */
export class Role {
  constructor(data = {}) {
    this.id = data.id || '';
    this.name = data.name || '';
    this.description = data.description || '';
    this.permissions = data.permissions || [];
  }

  /**
   * 获取角色类型样式
   * @returns {string} Element-UI的标签类型
   */
  getTypeClass() {
    const roleName = this.name.toLowerCase();
    if (roleName === 'admin') return 'danger';
    if (roleName === 'agent') return 'success';
    if (roleName === 'manager') return 'warning';
    return 'info';
  }
}

/**
 * 创建角色请求 - 对应RoleController的CreateRoleRequest
 */
export class CreateRoleRequest {
  constructor(data = {}) {
    this.name = data.name || '';
    this.description = data.description || '';
  }

  /**
   * 表单验证
   * @returns {Object} 包含isValid和message的对象
   */
  validate() {
    if (!this.name) {
      return { isValid: false, message: '角色名称不能为空' };
    }
    return { isValid: true, message: '' };
  }
}

/**
 * 更新角色请求 - 对应RoleController的UpdateRoleRequest
 */
export class UpdateRoleRequest {
  constructor(data = {}) {
    this.name = data.name || '';
    this.description = data.description || '';
  }

  /**
   * 表单验证
   * @returns {Object} 包含isValid和message的对象
   */
  validate() {
    if (!this.name) {
      return { isValid: false, message: '角色名称不能为空' };
    }
    return { isValid: true, message: '' };
  }
}

/**
 * 权限分配请求 - 对应RoleController的AssignPermissionsRequest
 */
export class AssignPermissionsRequest {
  constructor(data = {}) {
    this.permissionIds = data.permissionIds || [];
  }
}

/**
 * 权限模型 - 对应RoleController的PermissionDto
 */
export class Permission {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.name = data.name || '';
    this.code = data.code || '';
    this.parentId = data.parentId || null;
    this.children = data.children || [];
  }
}

/**
 * 将权限列表转换为树形结构
 * @param {Array} permissions 权限列表
 * @returns {Array} 树形结构的权限列表
 */
export function buildPermissionTree(permissions) {
  if (!permissions || permissions.length === 0) return [];
  
  const permissionMap = {};
  const result = [];

  // 构建Map
  permissions.forEach(permission => {
    const permObj = new Permission(permission);
    permissionMap[permObj.id] = {
      ...permObj,
      children: []
    };
  });

  // 构建树形结构
  permissions.forEach(permission => {
    const { parentId, id } = permission;
    if (parentId) {
      // 有父节点，添加到父节点的children中
      if (permissionMap[parentId]) {
        permissionMap[parentId].children.push(permissionMap[id]);
      }
    } else {
      // 没有父节点，作为根节点
      result.push(permissionMap[id]);
    }
  });

  return result;
}

/**
 * 获取所有权限ID的平铺数组（包括子权限）
 * @param {Array} permissions 树形结构的权限列表
 * @returns {Array} 权限ID列表
 */
export function getAllPermissionIds(permissions) {
  const ids = [];
  
  function traverse(perms) {
    if (!perms) return;
    
    perms.forEach(perm => {
      ids.push(perm.id);
      if (perm.children && perm.children.length > 0) {
        traverse(perm.children);
      }
    });
  }
  
  traverse(permissions);
  return ids;
} 