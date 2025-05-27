using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateSolution.ClientService.Models;
using RealEstateSolution.ClientService.Services;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.Database.Models;
using RealEstateSolution.ClientService.Dtos;
using System.Security.Claims;

namespace RealEstateSolution.ClientService.Controllers
{
    /// <summary>
    /// 客户管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        /// <summary>
        /// 获取当前用户ID
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("无效的用户身份");
            }
            return userId;
        }

        /// <summary>
        /// 获取客户列表
        /// </summary>
        [HttpGet]
        public async Task<ApiResponse<PagedList<Client>>> GetClients(
            [FromQuery] string name = null,
            [FromQuery] string phone = null,
            [FromQuery] ClientType? type = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            return await _clientService.GetClientsAsync(userId, name, phone, type, pageIndex, pageSize);
        }

        /// <summary>
        /// 获取客户详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ApiResponse<Client>> GetClient(int id)
        {
            var userId = GetCurrentUserId();
            return await _clientService.GetClientByIdAsync(id, userId);
        }

        /// <summary>
        /// 创建客户
        /// </summary>
        [HttpPost]
        public async Task<ApiResponse<Client>> CreateClient([FromBody] Client client)
        {
            var userId = GetCurrentUserId();
            return await _clientService.CreateClientAsync(client, userId);
        }

        /// <summary>
        /// 更新客户信息
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ApiResponse<Client>> UpdateClient(int id, [FromBody] Client client)
        {
            var userId = GetCurrentUserId();
            return await _clientService.UpdateClientAsync(id, client, userId);
        }

        /// <summary>
        /// 删除客户
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ApiResponse> DeleteClient(int id)
        {
            var userId = GetCurrentUserId();
            return await _clientService.DeleteClientAsync(id, userId);
        }

        /// <summary>
        /// 获取客户需求
        /// </summary>
        [HttpGet("{id}/requirements")]
        public async Task<ApiResponse<ClientRequirement>> GetClientRequirements(int id)
        {
            var userId = GetCurrentUserId();
            return await _clientService.GetClientRequirementsAsync(id, userId);
        }

        /// <summary>
        /// 更新客户需求
        /// </summary>
        [HttpPut("{id}/requirements")]
        public async Task<ApiResponse<ClientRequirement>> UpdateClientRequirements(int id, [FromBody] ClientRequirementDto requirementDto)
        {
            var userId = GetCurrentUserId();
            
            // 将DTO转换为领域模型
            var requirement = new ClientRequirement
            {
                ClientId = id,
                MinPrice = requirementDto.MinPrice,
                MaxPrice = requirementDto.MaxPrice,
                MinArea = requirementDto.MinArea,
                MaxArea = requirementDto.MaxArea,
                Location = requirementDto.Location,
                PropertyType = requirementDto.PropertyType,
                OtherRequirements = requirementDto.OtherRequirements
            };
            
            return await _clientService.UpdateClientRequirementsAsync(id, requirement, userId);
        }

        /// <summary>
        /// 获取客户统计数据
        /// </summary>
        [HttpGet("stats")]
        public async Task<ApiResponse<ClientStats>> GetClientStats()
        {
            var userId = GetCurrentUserId();
            return await _clientService.GetClientStatsAsync(userId);
        }
    }
}