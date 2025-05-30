using AutoMapper;
using RealEstateSolution.Common.Utils;
using RealEstateSolution.ContractService.Models;
using RealEstateSolution.ContractService.Repository;
using RealEstateSolution.Database.Models;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlAgilityPack;
using RealEstateSolution.Common.Repository;
using RealEstateSolution.ContractService.Data;

namespace RealEstateSolution.ContractService.Services;

/// <summary>
/// 合同模板服务实现类
/// </summary>
public class ContractTemplateService : IContractTemplateService
{
    private readonly IContractTemplateRepository _templateRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<ContractDbContext> _unitOfWork;

    public ContractTemplateService(
        IContractTemplateRepository templateRepository, 
        IMapper mapper,
        IUnitOfWork<ContractDbContext> unitOfWork)
    {
        _templateRepository = templateRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedList<ContractTemplateDto>>> GetTemplatesAsync(ContractTemplateQueryDto query)
    {
        try
        {
            var templates = await _templateRepository.SearchTemplatesAsync(
                query.Name,
                query.Type,
                query.IsActive);

            // 应用分页
            var totalCount = templates.Count();
            var pagedTemplates = templates
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            var templateDtos = _mapper.Map<List<ContractTemplateDto>>(pagedTemplates);

            var pagedResult = new PagedList<ContractTemplateDto>(
                templateDtos,
                totalCount,
                query.PageIndex,
                query.PageSize);

            return new ApiResponse<PagedList<ContractTemplateDto>>
            {
                Success = true,
                Data = pagedResult,
                Message = "获取模板列表成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<PagedList<ContractTemplateDto>>
            {
                Success = false,
                Message = $"获取模板列表失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractTemplateDto>> GetTemplateByIdAsync(int id)
    {
        try
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new ApiResponse<ContractTemplateDto>
                {
                    Success = false,
                    Message = "模板不存在"
                };
            }

            var templateDto = _mapper.Map<ContractTemplateDto>(template);
            return new ApiResponse<ContractTemplateDto>
            {
                Success = true,
                Data = templateDto,
                Message = "获取模板详情成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<ContractTemplateDto>
            {
                Success = false,
                Message = $"获取模板详情失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<List<ContractTemplateDto>>> GetActiveTemplatesByTypeAsync(ContractType? type = null)
    {
        try
        {
            IEnumerable<ContractTemplate> templates;
            
            if (type.HasValue)
            {
                templates = await _templateRepository.GetActiveTemplatesByTypeAsync(type.Value);
            }
            else
            {
                templates = await _templateRepository.SearchTemplatesAsync(null, null, true);
            }

            var templateDtos = _mapper.Map<List<ContractTemplateDto>>(templates);
            return new ApiResponse<List<ContractTemplateDto>>
            {
                Success = true,
                Data = templateDtos,
                Message = "获取模板列表成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<ContractTemplateDto>>
            {
                Success = false,
                Message = $"获取模板列表失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractTemplateDto>> CreateTemplateAsync(ContractTemplateCreateDto templateDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            // 检查模板名称是否已存在
            if (await _templateRepository.IsNameExistsAsync(templateDto.Name))
            {
                return new ApiResponse<ContractTemplateDto>
                {
                    Success = false,
                    Message = "模板名称已存在"
                };
            }

            var template = _mapper.Map<ContractTemplate>(templateDto);
            template.CreatedBy = userId;
            template.CreatedAt = DateTime.Now;
            template.UpdatedAt = DateTime.Now;
            template.FileSize = Encoding.UTF8.GetByteCount(template.Content);

            await _templateRepository.AddAsync(template);
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<ContractTemplateDto>(template);
            return new ApiResponse<ContractTemplateDto>
            {
                Success = true,
                Data = resultDto,
                Message = "创建模板成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ContractTemplateDto>
            {
                Success = false,
                Message = $"创建模板失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractTemplateDto>> UpdateTemplateAsync(int id, ContractTemplateCreateDto templateDto, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var existingTemplate = await _templateRepository.GetByIdAsync(id);
            if (existingTemplate == null)
            {
                return new ApiResponse<ContractTemplateDto>
                {
                    Success = false,
                    Message = "模板不存在"
                };
            }

            // 检查模板名称是否已存在（排除当前模板）
            if (await _templateRepository.IsNameExistsAsync(templateDto.Name, id))
            {
                return new ApiResponse<ContractTemplateDto>
                {
                    Success = false,
                    Message = "模板名称已存在"
                };
            }

            // 更新模板信息
            existingTemplate.Name = templateDto.Name;
            existingTemplate.Description = templateDto.Description;
            existingTemplate.Type = templateDto.Type;
            existingTemplate.Content = templateDto.Content;
            existingTemplate.Version = templateDto.Version;
            existingTemplate.IsActive = templateDto.IsActive;
            existingTemplate.UpdatedAt = DateTime.Now;
            existingTemplate.FileSize = Encoding.UTF8.GetByteCount(templateDto.Content);

            _templateRepository.Update(existingTemplate);
            await _unitOfWork.CommitAsync();

            var resultDto = _mapper.Map<ContractTemplateDto>(existingTemplate);
            return new ApiResponse<ContractTemplateDto>
            {
                Success = true,
                Data = resultDto,
                Message = "更新模板成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse<ContractTemplateDto>
            {
                Success = false,
                Message = $"更新模板失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse> DeleteTemplateAsync(int id, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "模板不存在"
                };
            }

            _templateRepository.Delete(template);
            await _unitOfWork.CommitAsync();

            return new ApiResponse
            {
                Success = true,
                Message = "删除模板成功"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse
            {
                Success = false,
                Message = $"删除模板失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse> UpdateTemplateStatusAsync(int id, bool isActive, string userId)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "模板不存在"
                };
            }

            template.IsActive = isActive;
            template.UpdatedAt = DateTime.Now;
            _templateRepository.Update(template);
            await _unitOfWork.CommitAsync();

            return new ApiResponse
            {
                Success = true,
                Message = $"模板已{(isActive ? "启用" : "禁用")}"
            };
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return new ApiResponse
            {
                Success = false,
                Message = $"更新模板状态失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<ContractTemplateDto>> ImportFromWordAsync(Stream fileStream, string fileName, ContractType type, string userId)
    {
        try
        {
            // 这里应该实现Word文档解析逻辑
            // 暂时返回一个示例实现
            using var reader = new StreamReader(fileStream);
            var content = await reader.ReadToEndAsync();

            var templateDto = new ContractTemplateCreateDto
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                Description = $"从Word文档 {fileName} 导入",
                Type = type,
                Content = content,
                Version = "1.0",
                IsActive = true
            };

            return await CreateTemplateAsync(templateDto, userId);
        }
        catch (Exception ex)
        {
            return new ApiResponse<ContractTemplateDto>
            {
                Success = false,
                Message = $"导入Word文档失败: {ex.Message}"
            };
        }
    }

    public async Task<ApiResponse<byte[]>> ExportToWordAsync(int id)
    {
        try
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
            {
                return new ApiResponse<byte[]>
                {
                    Success = false,
                    Message = "模板不存在"
                };
            }

            // 调试：检查模板内容
            Console.WriteLine($"模板ID: {template.Id}");
            Console.WriteLine($"模板名称: {template.Name}");
            Console.WriteLine($"模板内容长度: {template.Content?.Length ?? 0}");
            Console.WriteLine($"模板内容前100字符: {template.Content?.Substring(0, Math.Min(100, template.Content?.Length ?? 0))}");

            // 创建Word文档
            using var memoryStream = new MemoryStream();
            using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document))
            {
                // 添加主文档部分
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                // 转换HTML内容为Word段落
                ConvertHtmlToWordParagraphs(template.Content ?? string.Empty, body);
            }

            var bytes = memoryStream.ToArray();
            Console.WriteLine($"生成的Word文档大小: {bytes.Length} 字节");

            return new ApiResponse<byte[]>
            {
                Success = true,
                Data = bytes,
                Message = "导出成功"
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"导出失败异常: {ex.Message}");
            Console.WriteLine($"异常堆栈: {ex.StackTrace}");
            return new ApiResponse<byte[]>
            {
                Success = false,
                Message = $"导出失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 将HTML内容转换为Word段落
    /// </summary>
    private void ConvertHtmlToWordParagraphs(string htmlContent, Body body)
    {
        try
        {
            Console.WriteLine($"开始转换HTML内容，长度: {htmlContent?.Length ?? 0}");
            
            if (string.IsNullOrEmpty(htmlContent))
            {
                Console.WriteLine("HTML内容为空，创建默认段落");
                var emptyParagraph = new Paragraph();
                var emptyRun = new Run();
                emptyRun.AppendChild(new Text("模板内容为空"));
                emptyParagraph.AppendChild(emptyRun);
                body.AppendChild(emptyParagraph);
                return;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);
            
            Console.WriteLine($"HTML解析完成，根节点子节点数: {doc.DocumentNode.ChildNodes.Count}");

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                ProcessHtmlNode(node, body);
            }
            
            Console.WriteLine("HTML转换完成");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"HTML转换异常: {ex.Message}");
            // 如果HTML解析失败，直接作为纯文本处理
            var paragraph = new Paragraph();
            var run = new Run();
            run.AppendChild(new Text(htmlContent ?? "内容解析失败"));
            paragraph.AppendChild(run);
            body.AppendChild(paragraph);
        }
    }

    /// <summary>
    /// 处理HTML节点
    /// </summary>
    private void ProcessHtmlNode(HtmlNode node, Body body)
    {
        switch (node.NodeType)
        {
            case HtmlNodeType.Text:
                if (!string.IsNullOrWhiteSpace(node.InnerText))
                {
                    var paragraph = new Paragraph();
                    var run = new Run();
                    run.AppendChild(new Text(node.InnerText.Trim()));
                    paragraph.AppendChild(run);
                    body.AppendChild(paragraph);
                }
                break;

            case HtmlNodeType.Element:
                ProcessHtmlElement(node, body);
                break;
        }
    }

    /// <summary>
    /// 处理HTML元素
    /// </summary>
    private void ProcessHtmlElement(HtmlNode element, Body body)
    {
        switch (element.Name.ToLower())
        {
            case "h1":
            case "h2":
            case "h3":
            case "h4":
            case "h5":
            case "h6":
                CreateHeadingParagraph(element, body, element.Name);
                break;

            case "p":
                CreateParagraph(element, body);
                break;

            case "br":
                CreateEmptyParagraph(body);
                break;

            case "div":
                ProcessDivElement(element, body);
                break;

            case "strong":
            case "b":
                CreateBoldParagraph(element, body);
                break;

            default:
                // 对于其他元素，递归处理子节点
                foreach (var child in element.ChildNodes)
                {
                    ProcessHtmlNode(child, body);
                }
                break;
        }
    }

    /// <summary>
    /// 创建标题段落
    /// </summary>
    private void CreateHeadingParagraph(HtmlNode element, Body body, string headingLevel)
    {
        var paragraph = new Paragraph();
        var paragraphProperties = new ParagraphProperties();
        
        // 设置标题样式
        var paragraphStyleId = new ParagraphStyleId() { Val = "Heading" + headingLevel.Substring(1) };
        paragraphProperties.AppendChild(paragraphStyleId);
        
        // 居中对齐（如果有text-align: center样式）
        var style = element.GetAttributeValue("style", "");
        if (style.Contains("text-align: center") || style.Contains("text-align:center"))
        {
            var justification = new Justification() { Val = JustificationValues.Center };
            paragraphProperties.AppendChild(justification);
        }
        
        paragraph.AppendChild(paragraphProperties);

        var run = new Run();
        var runProperties = new RunProperties();
        runProperties.AppendChild(new Bold());
        
        // 设置字体大小
        var fontSize = headingLevel switch
        {
            "h1" => "32",
            "h2" => "28",
            "h3" => "24",
            _ => "20"
        };
        runProperties.AppendChild(new FontSize() { Val = fontSize });
        
        run.AppendChild(runProperties);
        run.AppendChild(new Text(element.InnerText));
        paragraph.AppendChild(run);
        body.AppendChild(paragraph);
    }

    /// <summary>
    /// 创建普通段落
    /// </summary>
    private void CreateParagraph(HtmlNode element, Body body)
    {
        var paragraph = new Paragraph();
        
        // 检查样式
        var style = element.GetAttributeValue("style", "");
        if (style.Contains("text-align: center") || style.Contains("text-align:center"))
        {
            var paragraphProperties = new ParagraphProperties();
            var justification = new Justification() { Val = JustificationValues.Center };
            paragraphProperties.AppendChild(justification);
            paragraph.AppendChild(paragraphProperties);
        }

        ProcessInlineElements(element, paragraph);
        body.AppendChild(paragraph);
    }

    /// <summary>
    /// 处理内联元素
    /// </summary>
    private void ProcessInlineElements(HtmlNode element, Paragraph paragraph)
    {
        foreach (var child in element.ChildNodes)
        {
            if (child.NodeType == HtmlNodeType.Text)
            {
                if (!string.IsNullOrWhiteSpace(child.InnerText))
                {
                    var run = new Run();
                    run.AppendChild(new Text(child.InnerText));
                    paragraph.AppendChild(run);
                }
            }
            else if (child.NodeType == HtmlNodeType.Element)
            {
                var run = new Run();
                var runProperties = new RunProperties();

                switch (child.Name.ToLower())
                {
                    case "strong":
                    case "b":
                        runProperties.AppendChild(new Bold());
                        break;
                    case "em":
                    case "i":
                        runProperties.AppendChild(new Italic());
                        break;
                }

                if (runProperties.HasChildren)
                {
                    run.AppendChild(runProperties);
                }

                run.AppendChild(new Text(child.InnerText));
                paragraph.AppendChild(run);
            }
        }
    }

    /// <summary>
    /// 创建粗体段落
    /// </summary>
    private void CreateBoldParagraph(HtmlNode element, Body body)
    {
        var paragraph = new Paragraph();
        var run = new Run();
        var runProperties = new RunProperties();
        runProperties.AppendChild(new Bold());
        run.AppendChild(runProperties);
        run.AppendChild(new Text(element.InnerText));
        paragraph.AppendChild(run);
        body.AppendChild(paragraph);
    }

    /// <summary>
    /// 处理div元素
    /// </summary>
    private void ProcessDivElement(HtmlNode element, Body body)
    {
        var style = element.GetAttributeValue("style", "");
        
        // 检查是否是flex布局（用于签名区域）
        if (style.Contains("display: flex") || style.Contains("display:flex"))
        {
            CreateFlexParagraph(element, body);
        }
        else
        {
            // 普通div，递归处理子节点
            foreach (var child in element.ChildNodes)
            {
                ProcessHtmlNode(child, body);
            }
        }
    }

    /// <summary>
    /// 创建flex布局段落（用于签名区域）
    /// </summary>
    private void CreateFlexParagraph(HtmlNode element, Body body)
    {
        var table = new Table();
        var tableProperties = new TableProperties();
        var tableWidth = new TableWidth() { Width = "5000", Type = TableWidthUnitValues.Pct };
        tableProperties.AppendChild(tableWidth);
        table.AppendChild(tableProperties);

        var tableRow = new TableRow();
        
        foreach (var child in element.ChildNodes)
        {
            if (child.NodeType == HtmlNodeType.Element && child.Name.ToLower() == "div")
            {
                var tableCell = new TableCell();
                var cellParagraph = new Paragraph();
                
                foreach (var grandChild in child.ChildNodes)
                {
                    if (grandChild.NodeType == HtmlNodeType.Element && grandChild.Name.ToLower() == "p")
                    {
                        var run = new Run();
                        var runProperties = new RunProperties();
                        
                        if (grandChild.InnerText.Contains("签字") || grandChild.InnerText.Contains("日期"))
                        {
                            runProperties.AppendChild(new Bold());
                        }
                        
                        run.AppendChild(runProperties);
                        run.AppendChild(new Text(grandChild.InnerText));
                        cellParagraph.AppendChild(run);
                    }
                }
                
                tableCell.AppendChild(cellParagraph);
                tableRow.AppendChild(tableCell);
            }
        }
        
        table.AppendChild(tableRow);
        body.AppendChild(table);
    }

    /// <summary>
    /// 创建空段落
    /// </summary>
    private void CreateEmptyParagraph(Body body)
    {
        var paragraph = new Paragraph();
        var run = new Run();
        run.AppendChild(new Text(""));
        paragraph.AppendChild(run);
        body.AppendChild(paragraph);
    }

    public async Task<ApiResponse<object>> GetTemplateStatsAsync()
    {
        try
        {
            var (total, active, inactive) = await _templateRepository.GetTemplateStatsAsync();

            var stats = new
            {
                Total = total,
                Active = active,
                Inactive = inactive,
                SaleTemplates = await _templateRepository.GetActiveTemplatesByTypeAsync(ContractType.Sale),
                RentTemplates = await _templateRepository.GetActiveTemplatesByTypeAsync(ContractType.Rent),
                CommissionTemplates = await _templateRepository.GetActiveTemplatesByTypeAsync(ContractType.Commission)
            };

            return new ApiResponse<object>
            {
                Success = true,
                Data = new
                {
                    stats.Total,
                    stats.Active,
                    stats.Inactive,
                    SaleCount = stats.SaleTemplates.Count(),
                    RentCount = stats.RentTemplates.Count(),
                    CommissionCount = stats.CommissionTemplates.Count()
                },
                Message = "获取统计信息成功"
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<object>
            {
                Success = false,
                Message = $"获取统计信息失败: {ex.Message}"
            };
        }
    }
} 