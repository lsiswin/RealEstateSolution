/**
 * 房源模型 - 对应PropertyController的Property
 */
export class Property {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.title = data.title || '';
    this.description = data.description || '';
    this.price = data.price || 0;
    this.area = data.area || 0;
    this.address = data.address || '';
    this.type = data.type !== undefined ? data.type : 0; // PropertyType枚举
    this.decoration = data.decoration !== undefined ? data.decoration : 0; // Decoration枚举
    this.orientation = data.orientation !== undefined ? data.orientation : 0; // Orientation枚举
    this.floor = data.floor || 0;
    this.totalFloors = data.totalFloors || 0;
    this.rooms = data.rooms || 0;
    this.bathrooms = data.bathrooms || 0;
    this.status = data.status !== undefined ? data.status : 0; // PropertyStatus枚举
    this.ownerId = data.ownerId || 0;
    this.createTime = data.createTime ? new Date(data.createTime) : new Date();
    this.updateTime = data.updateTime ? new Date(data.updateTime) : new Date();
    this.images = data.images || [];
  }

  /**
   * 获取房源主图
   * @returns {string} 主图URL或默认图片
   */
  getMainImage() {
    if (!this.images || this.images.length === 0) {
      return '/images/property-default.jpg';
    }
    
    const mainImage = this.images.find(img => img.isMain);
    return mainImage ? mainImage.filePath : this.images[0].filePath;
  }

  /**
   * 获取格式化后的房源价格
   * @returns {string} 格式化后的价格
   */
  getFormattedPrice() {
    return this.price.toLocaleString('zh-CN') + '元';
  }

  /**
   * 获取房源状态标签类型
   * @returns {string} Element-UI的标签类型
   */
  getStatusType() {
    return PropertyStatus.getStatusType(this.status);
  }

  /**
   * 获取房源状态文本
   * @returns {string} 状态文本
   */
  getStatusText() {
    return PropertyStatus.toString(this.status);
  }

  /**
   * 获取房源类型文本
   * @returns {string} 类型文本
   */
  getTypeText() {
    return PropertyType.toString(this.type);
  }

  /**
   * 获取装修状态文本
   * @returns {string} 装修状态文本
   */
  getDecorationText() {
    return Decoration.toString(this.decoration);
  }

  /**
   * 获取朝向文本
   * @returns {string} 朝向文本
   */
  getOrientationText() {
    return Orientation.toString(this.orientation);
  }

  /**
   * 验证房源数据
   * @returns {Object} 包含isValid和message的对象
   */
  validate() {
    if (!this.title) {
      return { isValid: false, message: '房源标题不能为空' };
    }
    if (!this.address) {
      return { isValid: false, message: '房源地址不能为空' };
    }
    if (this.price <= 0) {
      return { isValid: false, message: '房源价格必须大于0' };
    }
    if (this.area <= 0) {
      return { isValid: false, message: '房源面积必须大于0' };
    }
    
    return { isValid: true, message: '' };
  }
}

/**
 * 房源图片模型
 */
export class PropertyImage {
  constructor(data = {}) {
    this.id = data.id || 0;
    this.propertyId = data.propertyId || 0;
    this.fileName = data.fileName || '';
    this.filePath = data.filePath || '';
    this.fileSize = data.fileSize || 0;
    this.fileType = data.fileType || '';
    this.uploadedAt = data.uploadedAt ? new Date(data.uploadedAt) : new Date();
    this.uploadedBy = data.uploadedBy || 0;
    this.isMain = data.isMain || false;
  }

  /**
   * 获取图片尺寸描述
   * @returns {string} 图片尺寸描述
   */
  getSizeDescription() {
    if (this.fileSize < 1024) {
      return `${this.fileSize}B`;
    } else if (this.fileSize < 1024 * 1024) {
      return `${(this.fileSize / 1024).toFixed(2)}KB`;
    } else {
      return `${(this.fileSize / (1024 * 1024)).toFixed(2)}MB`;
    }
  }
}

/**
 * 房源搜索参数
 */
export class PropertySearchParams {
  constructor(data = {}) {
    this.type = data.type !== undefined ? data.type : null;
    this.minPrice = data.minPrice || null;
    this.maxPrice = data.maxPrice || null;
    this.minArea = data.minArea || null;
    this.maxArea = data.maxArea || null;
    this.status = data.status !== undefined ? data.status : null;
    this.keyword = data.keyword || '';
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

    if (this.type !== null) params.type = this.type;
    if (this.minPrice !== null) params.minPrice = this.minPrice;
    if (this.maxPrice !== null) params.maxPrice = this.maxPrice;
    if (this.minArea !== null) params.minArea = this.minArea;
    if (this.maxArea !== null) params.maxArea = this.maxArea;
    if (this.status !== null) params.status = this.status;
    if (this.keyword) params.keyword = this.keyword;

    return params;
  }
}

/**
 * 房源统计数据模型 - 对应PropertyController的PropertyStats
 */
export class PropertyStats {
  constructor(data = {}) {
    this.total = data.total || 0;
    this.forSale = data.forSale || 0;
    this.sold = data.sold || 0;
    this.forRent = data.forRent || 0;
    this.rented = data.rented || 0;
    this.typeDistribution = data.typeDistribution || {};
  }

  /**
   * 获取各类型分布的图表数据
   * @returns {Array} 适用于图表的数据数组
   */
  getTypeChartData() {
    const data = [];
    for (const [type, count] of Object.entries(this.typeDistribution)) {
      data.push({
        name: PropertyType.toString(parseInt(type)),
        value: count
      });
    }
    return data;
  }

  /**
   * 获取房源状态分布的图表数据
   * @returns {Array} 适用于图表的数据数组
   */
  getStatusChartData() {
    return [
      { name: '在售', value: this.forSale },
      { name: '已售', value: this.sold },
      { name: '出租', value: this.forRent },
      { name: '已租', value: this.rented }
    ];
  }
}

/**
 * 房源类型枚举 - 对应后端的PropertyType枚举
 */
export const PropertyType = {
  Apartment: 0,
  House: 1,
  Villa: 2,
  Office: 3,
  Shop: 4,
  Industrial: 5,
  Land: 6,
  
  /**
   * 获取房源类型名称
   * @param {number} type 房源类型值
   * @returns {string} 房源类型名称
   */
  toString: function(type) {
    switch (type) {
      case this.Apartment: return '公寓';
      case this.House: return '住宅';
      case this.Villa: return '别墅';
      case this.Office: return '写字楼';
      case this.Shop: return '商铺';
      case this.Industrial: return '工业用地';
      case this.Land: return '土地';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有房源类型列表
   * @returns {Array} 房源类型列表
   */
  getOptions: function() {
    return [
      { value: this.Apartment, label: '公寓' },
      { value: this.House, label: '住宅' },
      { value: this.Villa, label: '别墅' },
      { value: this.Office, label: '写字楼' },
      { value: this.Shop, label: '商铺' },
      { value: this.Industrial, label: '工业用地' },
      { value: this.Land, label: '土地' }
    ];
  }
};

/**
 * 房源状态枚举 - 对应后端的PropertyStatus枚举
 */
export const PropertyStatus = {
  Draft: 0,
  ForSale: 1,
  Sold: 2,
  ForRent: 3,
  Rented: 4,
  Offline: 5,
  
  /**
   * 获取房源状态名称
   * @param {number} status 房源状态值
   * @returns {string} 房源状态名称
   */
  toString: function(status) {
    switch (status) {
      case this.Draft: return '草稿';
      case this.ForSale: return '在售';
      case this.Sold: return '已售';
      case this.ForRent: return '出租';
      case this.Rented: return '已租';
      case this.Offline: return '下架';
      default: return '未知';
    }
  },
  
  /**
   * 获取状态对应的Element-UI标签类型
   * @param {number} status 房源状态值
   * @returns {string} 标签类型
   */
  getStatusType: function(status) {
    switch (status) {
      case this.Draft: return 'info';
      case this.ForSale: return 'success';
      case this.Sold: return 'warning';
      case this.ForRent: return 'primary';
      case this.Rented: return 'warning';
      case this.Offline: return 'danger';
      default: return 'info';
    }
  },
  
  /**
   * 获取所有房源状态列表
   * @returns {Array} 房源状态列表
   */
  getOptions: function() {
    return [
      { value: this.Draft, label: '草稿' },
      { value: this.ForSale, label: '在售' },
      { value: this.Sold, label: '已售' },
      { value: this.ForRent, label: '出租' },
      { value: this.Rented, label: '已租' },
      { value: this.Offline, label: '下架' }
    ];
  }
};

/**
 * 装修状态枚举 - 对应后端的Decoration枚举
 */
export const Decoration = {
  None: 0,
  Simple: 1,
  Medium: 2,
  Luxury: 3,
  
  /**
   * 获取装修状态名称
   * @param {number} decoration 装修状态值
   * @returns {string} 装修状态名称
   */
  toString: function(decoration) {
    switch (decoration) {
      case this.None: return '毛坯';
      case this.Simple: return '简装';
      case this.Medium: return '中装';
      case this.Luxury: return '豪装';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有装修状态列表
   * @returns {Array} 装修状态列表
   */
  getOptions: function() {
    return [
      { value: this.None, label: '毛坯' },
      { value: this.Simple, label: '简装' },
      { value: this.Medium, label: '中装' },
      { value: this.Luxury, label: '豪装' }
    ];
  }
};

/**
 * 朝向枚举 - 对应后端的Orientation枚举
 */
export const Orientation = {
  East: 0,
  South: 1,
  West: 2,
  North: 3,
  Southeast: 4,
  Southwest: 5,
  Northeast: 6,
  Northwest: 7,
  
  /**
   * 获取朝向名称
   * @param {number} orientation 朝向值
   * @returns {string} 朝向名称
   */
  toString: function(orientation) {
    switch (orientation) {
      case this.East: return '东';
      case this.South: return '南';
      case this.West: return '西';
      case this.North: return '北';
      case this.Southeast: return '东南';
      case this.Southwest: return '西南';
      case this.Northeast: return '东北';
      case this.Northwest: return '西北';
      default: return '未知';
    }
  },
  
  /**
   * 获取所有朝向列表
   * @returns {Array} 朝向列表
   */
  getOptions: function() {
    return [
      { value: this.East, label: '东' },
      { value: this.South, label: '南' },
      { value: this.West, label: '西' },
      { value: this.North, label: '北' },
      { value: this.Southeast, label: '东南' },
      { value: this.Southwest, label: '西南' },
      { value: this.Northeast, label: '东北' },
      { value: this.Northwest, label: '西北' }
    ];
  }
}; 