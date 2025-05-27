/**
 * 客户模型 - 对应ClientController的Client
 */
export class Client {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.name = data.name || '';
    this.phone = data.phone || '';
    this.type = data.type !== undefined ? data.type : 0; // ClientType枚举
    this.gender = data.gender !== undefined ? data.gender : 0; // Gender枚举
    this.age = data.age || 0;
    this.occupation = data.occupation || '';
    this.email = data.email || '';
    this.address = data.address || '';
    this.source = data.source || '';
    this.remark = data.remark || '';
    this.agentId = data.agentId || 0;
    this.createTime = data.createTime ? new Date(data.createTime) : new Date();
    this.updateTime = data.updateTime ? new Date(data.updateTime) : new Date();
  }

  /**
   * 获取客户类型文本
   * @returns {string} 客户类型文本
   */
  getTypeText() {
    return ClientType.toString(this.type);
  }

  /**
   * 获取客户类型标签样式
   * @returns {string} 标签样式
   */
  getTypeClass() {
    switch (this.type) {
      case ClientType.Buyer: return 'success';
      case ClientType.Seller: return 'warning';
      case ClientType.Renter: return 'info';
      case ClientType.Landlord: return 'primary';
      default: return 'info';
    }
  }

  /**
   * 获取性别文本
   * @returns {string} 性别文本
   */
  getGenderText() {
    return Gender.toString(this.gender);
  }

  /**
   * 获取客户头像
   * @returns {string} 头像URL
   */
  getAvatar() {
    // 根据性别返回不同的默认头像
    if (this.gender === Gender.Female) {
      return '/images/avatar-female.png';
    } else if (this.gender === Gender.Male) {
      return '/images/avatar-male.png';
    } else {
      return '/images/avatar-default.png';
    }
  }

  /**
   * 验证客户数据
   * @returns {Object} 包含isValid和message的对象
   */
  validate() {
    if (!this.name) {
      return { isValid: false, message: '客户姓名不能为空' };
    }
    if (!this.phone) {
      return { isValid: false, message: '客户电话不能为空' };
    }
    // 简单的手机号验证
    if (!/^1[3-9]\d{9}$/.test(this.phone)) {
      return { isValid: false, message: '请输入有效的手机号' };
    }
    
    return { isValid: true, message: '' };
  }
}

/**
 * 客户需求模型 - 对应ClientController的ClientRequirement
 */
export class ClientRequirement {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.clientId = data.clientId || 0;
    this.propertyType = data.propertyType !== undefined ? data.propertyType : 0;
    this.minPrice = data.minPrice || 0;
    this.maxPrice = data.maxPrice || 0;
    this.minArea = data.minArea || 0;
    this.maxArea = data.maxArea || 0;
    this.locations = data.locations || [];
    this.minRooms = data.minRooms || 0;
    this.minBathrooms = data.minBathrooms || 0;
    this.decoration = data.decoration !== undefined ? data.decoration : 0;
    this.purpose = data.purpose !== undefined ? data.purpose : 0; // 购买目的：0=自住，1=投资
    this.timeline = data.timeline !== undefined ? data.timeline : 0; // 0=近期，1=三个月内，2=半年内，3=一年内
    this.otherRequirements = data.otherRequirements || '';
    this.createTime = data.createTime ? new Date(data.createTime) : new Date();
    this.updateTime = data.updateTime ? new Date(data.updateTime) : new Date();
  }

  /**
   * 获取价格范围文本
   * @returns {string} 价格范围文本
   */
  getPriceRangeText() {
    if (this.minPrice > 0 && this.maxPrice > 0) {
      return `${this.minPrice.toLocaleString('zh-CN')} - ${this.maxPrice.toLocaleString('zh-CN')}元`;
    } else if (this.minPrice > 0) {
      return `${this.minPrice.toLocaleString('zh-CN')}元以上`;
    } else if (this.maxPrice > 0) {
      return `${this.maxPrice.toLocaleString('zh-CN')}元以内`;
    } else {
      return '价格不限';
    }
  }

  /**
   * 获取面积范围文本
   * @returns {string} 面积范围文本
   */
  getAreaRangeText() {
    if (this.minArea > 0 && this.maxArea > 0) {
      return `${this.minArea} - ${this.maxArea}平方米`;
    } else if (this.minArea > 0) {
      return `${this.minArea}平方米以上`;
    } else if (this.maxArea > 0) {
      return `${this.maxArea}平方米以内`;
    } else {
      return '面积不限';
    }
  }

  /**
   * 获取房间需求文本
   * @returns {string} 房间需求文本
   */
  getRoomsText() {
    if (this.minRooms > 0) {
      return `${this.minRooms}室以上`;
    }
    return '不限';
  }

  /**
   * 获取购买目的文本
   * @returns {string} 购买目的文本
   */
  getPurposeText() {
    return this.purpose === 0 ? '自住' : '投资';
  }

  /**
   * 获取时间线文本
   * @returns {string} 时间线文本
   */
  getTimelineText() {
    switch (this.timeline) {
      case 0: return '近期';
      case 1: return '三个月内';
      case 2: return '半年内';
      case 3: return '一年内';
      default: return '不限';
    }
  }

  /**
   * 验证客户需求数据
   * @returns {Object} 包含isValid和message的对象
   */
  validate() {
    if (this.maxPrice > 0 && this.minPrice > this.maxPrice) {
      return { isValid: false, message: '最低价格不能高于最高价格' };
    }
    if (this.maxArea > 0 && this.minArea > this.maxArea) {
      return { isValid: false, message: '最小面积不能大于最大面积' };
    }
    
    return { isValid: true, message: '' };
  }
}

/**
 * 客户搜索参数
 */
export class ClientSearchParams {
  constructor(data = {}) {
    this.name = data.name || '';
    this.phone = data.phone || '';
    this.type = data.type !== undefined ? data.type : null;
    this.pageIndex = data.pageIndex || 1;
    this.pageSize = data.pageSize || 10;
  }

  /**
   * 转换为请求参数对象
   * @returns {Object} 请求参数对象
   */
  toRequestParams() {
    const params = {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize
    };

    if (this.name) params.name = this.name;
    if (this.phone) params.phone = this.phone;
    if (this.type !== null) params.type = this.type;

    return params;
  }
}

/**
 * 客户统计数据模型 - 对应ClientController的ClientStats
 */
export class ClientStats {
  constructor(data = {}) {
    this.total = data.total || 0;
    this.newToday = data.newToday || 0;
    this.newThisWeek = data.newThisWeek || 0;
    this.newThisMonth = data.newThisMonth || 0;
    this.typeDistribution = data.typeDistribution || {};
    this.sourceDistribution = data.sourceDistribution || {};
  }

  /**
   * 获取客户类型分布的图表数据
   * @returns {Array} 适用于图表的数据数组
   */
  getTypeChartData() {
    const data = [];
    for (const [type, count] of Object.entries(this.typeDistribution)) {
      data.push({
        name: ClientType.toString(parseInt(type)),
        value: count
      });
    }
    return data;
  }

  /**
   * 获取客户来源分布的图表数据
   * @returns {Array} 适用于图表的数据数组
   */
  getSourceChartData() {
    const data = [];
    for (const [source, count] of Object.entries(this.sourceDistribution)) {
      data.push({
        name: source || '未知',
        value: count
      });
    }
    return data;
  }
}

/**
 * 客户类型枚举 - 对应后端的ClientType枚举
 */
export const ClientType = {
  Buyer: 0,
  Seller: 1,
  Renter: 2,
  Landlord: 3,
  
  /**
   * 获取客户类型名称
   * @param {number} type 客户类型值
   * @returns {string} 客户类型名称
   */
  toString: function(type) {
    switch (type) {
      case this.Buyer: return '买家';
      case this.Seller: return '卖家';
      case this.Renter: return '租客';
      case this.Landlord: return '房东';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有客户类型列表
   * @returns {Array} 客户类型列表
   */
  getOptions: function() {
    return [
      { value: this.Buyer, label: '买家' },
      { value: this.Seller, label: '卖家' },
      { value: this.Renter, label: '租客' },
      { value: this.Landlord, label: '房东' }
    ];
  }
};

/**
 * 性别枚举 - 对应后端的Gender枚举
 */
export const Gender = {
  Unknown: 0,
  Male: 1,
  Female: 2,
  
  /**
   * 获取性别名称
   * @param {number} gender 性别值
   * @returns {string} 性别名称
   */
  toString: function(gender) {
    switch (gender) {
      case this.Unknown: return '未知';
      case this.Male: return '男';
      case this.Female: return '女';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有性别列表
   * @returns {Array} 性别列表
   */
  getOptions: function() {
    return [
      { value: this.Male, label: '男' },
      { value: this.Female, label: '女' },
      { value: this.Unknown, label: '未知' }
    ];
  }
};

/**
 * 购买目的枚举
 */
export const PurposeType = {
  SelfUse: 0,
  Investment: 1,
  
  /**
   * 获取购买目的名称
   * @param {number} purpose 购买目的值
   * @returns {string} 购买目的名称
   */
  toString: function(purpose) {
    switch (purpose) {
      case this.SelfUse: return '自住';
      case this.Investment: return '投资';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有购买目的列表
   * @returns {Array} 购买目的列表
   */
  getOptions: function() {
    return [
      { value: this.SelfUse, label: '自住' },
      { value: this.Investment, label: '投资' }
    ];
  }
};

/**
 * 时间线枚举
 */
export const TimelineType = {
  Soon: 0,
  ThreeMonths: 1,
  SixMonths: 2,
  OneYear: 3,
  
  /**
   * 获取时间线名称
   * @param {number} timeline 时间线值
   * @returns {string} 时间线名称
   */
  toString: function(timeline) {
    switch (timeline) {
      case this.Soon: return '近期';
      case this.ThreeMonths: return '三个月内';
      case this.SixMonths: return '半年内';
      case this.OneYear: return '一年内';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有时间线列表
   * @returns {Array} 时间线列表
   */
  getOptions: function() {
    return [
      { value: this.Soon, label: '近期' },
      { value: this.ThreeMonths, label: '三个月内' },
      { value: this.SixMonths, label: '半年内' },
      { value: this.OneYear, label: '一年内' }
    ];
  }
}; 