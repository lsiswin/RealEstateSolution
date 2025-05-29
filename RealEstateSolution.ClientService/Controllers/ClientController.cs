using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.ClientService.Services;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.ClientService.Dtos;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace RealEstateSolution.ClientService.Controllers
{
    /// <summary>
    /// 客户管理控制器
    /// 提供客户信息的增删改查、需求管理和统计功能的API接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "broker,admin")] // 需要身份验证，但不验证具体用户ID
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly ILogger<ClientController> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="clientService">客户服务接口</param>
        /// <param name="logger">日志记录器</param>
        public ClientController(IClientService clientService, ILogger<ClientController> logger)
        {
            _clientService = clientService;
            _logger = logger;
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        private string GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("无效的用户身份");
            }
            return userId;
        }

        /// <summary>
        /// 获取客户列表
        /// 支持按姓名、电话、类型进行筛选，并提供分页功能
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>分页的客户列表</returns>
        /// <response code="200">成功返回客户列表</response>
        /// <response code="400">请求参数无效</response>
        /// <response code="401">未授权访问</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedList<ClientDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<PagedList<ClientDto>>> GetClients([FromQuery] ClientQueryDto query)
        {
            _logger.LogInformation("获取客户列表 - 查询参数:{@Query}", query);
            
            return await _clientService.GetClientsAsync(query);
        }

        /// <summary>
        /// 根据ID获取客户详细信息
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <returns>客户详细信息</returns>
        /// <response code="200">成功返回客户信息</response>
        /// <response code="404">客户不存在</response>
        /// <response code="401">未授权访问</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ClientDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<ClientDto>> GetClient(int id)
        {
            _logger.LogInformation("获取客户详情 - ID:{ClientId}", id);
            
            return await _clientService.GetClientByIdAsync(id);
        }

        /// <summary>
        /// 创建新客户
        /// 系统会自动设置创建时间、更新时间和默认状态
        /// </summary>
        /// <param name="clientDto">客户信息DTO</param>
        /// <returns>创建成功的客户信息</returns>
        /// <response code="200">客户创建成功</response>
        /// <response code="400">客户信息无效或电话号码已存在</response>
        /// <response code="401">未授权访问</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ClientDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<ClientDto>> CreateClient([FromBody] ClientDto clientDto)
        {
            _logger.LogInformation("创建客户 - 姓名:{Name}, 电话:{Phone}", clientDto?.Name, clientDto?.Phone);
            
            var userId = GetCurrentUserId();
            return await _clientService.CreateClientAsync(clientDto, userId);
        }

        /// <summary>
        /// 更新客户信息
        /// 只更新允许修改的字段，系统会自动更新修改时间
        /// </summary>
        /// <param name="id">要更新的客户ID</param>
        /// <param name="clientDto">新的客户信息DTO</param>
        /// <returns>更新后的客户信息</returns>
        /// <response code="200">客户信息更新成功</response>
        /// <response code="400">客户信息无效或电话号码冲突</response>
        /// <response code="404">客户不存在</response>
        /// <response code="401">未授权访问</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ClientDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<ClientDto>> UpdateClient(int id, [FromBody] ClientDto clientDto)
        {
            _logger.LogInformation("更新客户信息 - ID:{ClientId}, 姓名:{Name}, 电话:{Phone}", 
                id, clientDto?.Name, clientDto?.Phone);
            
            var userId = GetCurrentUserId();
            return await _clientService.UpdateClientAsync(id, clientDto, userId);
        }

        /// <summary>
        /// 删除客户
        /// 这是物理删除操作，删除后数据无法恢复，同时会删除关联的需求信息
        /// </summary>
        /// <param name="id">要删除的客户ID</param>
        /// <returns>删除操作结果</returns>
        /// <response code="200">客户删除成功</response>
        /// <response code="404">客户不存在</response>
        /// <response code="401">未授权访问</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse> DeleteClient(int id)
        {
            _logger.LogInformation("删除客户 - ID:{ClientId}", id);
            
            var userId = GetCurrentUserId();
            return await _clientService.DeleteClientAsync(id, userId);
        }

        /// <summary>
        /// 获取客户需求信息
        /// 包含客户的房源需求，如价格范围、面积要求、位置偏好等
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <returns>客户需求详细信息</returns>
        /// <response code="200">成功返回客户需求信息</response>
        /// <response code="404">客户不存在或尚未设置需求</response>
        /// <response code="401">未授权访问</response>
        [HttpGet("{id}/requirements")]
        [ProducesResponseType(typeof(ApiResponse<ClientRequirementDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<ClientRequirementDto>> GetClientRequirements(int id)
        {
            _logger.LogInformation("获取客户需求 - 客户ID:{ClientId}", id);
            
            return await _clientService.GetClientRequirementsAsync(id);
        }

        /// <summary>
        /// 更新或创建客户需求信息
        /// 如果客户已有需求信息则更新，否则创建新的需求记录
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <param name="requirementDto">客户需求信息DTO</param>
        /// <returns>更新后的客户需求信息</returns>
        /// <response code="200">客户需求更新成功</response>
        /// <response code="400">需求信息无效（如价格范围或面积范围不合理）</response>
        /// <response code="404">客户不存在</response>
        /// <response code="401">未授权访问</response>
        [HttpPut("{id}/requirements")]
        [ProducesResponseType(typeof(ApiResponse<ClientRequirementDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse), 400)]
        [ProducesResponseType(typeof(ApiResponse), 404)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<ClientRequirementDto>> UpdateClientRequirements(int id, [FromBody] ClientRequirementDto requirementDto)
        {
            _logger.LogInformation("更新客户需求 - 客户ID:{ClientId}, 价格范围:{MinPrice}-{MaxPrice}, 面积范围:{MinArea}-{MaxArea}", 
                id, requirementDto?.MinPrice, requirementDto?.MaxPrice, requirementDto?.MinArea, requirementDto?.MaxArea);
            
            if (requirementDto == null)
            {
                return ApiResponse<ClientRequirementDto>.Error("需求信息不能为空");
            }
            
            var userId = GetCurrentUserId();
            return await _clientService.UpdateClientRequirementsAsync(id, requirementDto, userId);
        }

        /// <summary>
        /// 获取客户统计数据
        /// 提供全面的客户数据统计，包括总数、状态分布、类型分布和时间趋势
        /// </summary>
        /// <returns>客户相关的统计信息</returns>
        /// <response code="200">成功返回统计数据</response>
        /// <response code="401">未授权访问</response>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<ClientStatsDto>), 200)]
        [ProducesResponseType(401)]
        public async Task<ApiResponse<ClientStatsDto>> GetClientStats()
        {
            _logger.LogInformation("获取客户统计数据");
            
            return await _clientService.GetClientStatsAsync();
        }
    }
}