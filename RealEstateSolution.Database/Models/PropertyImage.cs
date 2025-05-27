using System;

namespace RealEstateSolution.Database.Models
{
    /// <summary>
    /// 房源图片
    /// </summary>
    public class PropertyImage
    {
        /// <summary>
        /// 图片ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 房源ID
        /// </summary>
        public int PropertyId { get; set; }

        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 是否为主图
        /// </summary>
        public bool IsMain { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadedAt { get; set; }

        /// <summary>
        /// 上传人ID
        /// </summary>
        public int UploadedBy { get; set; }
    }
}