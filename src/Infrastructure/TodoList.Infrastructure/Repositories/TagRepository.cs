using System.Text;
using AutoMapper;
using Dapper;
using TodoList.Application.DTOs.Tag;
using TodoList.Application.ports.Repositories;
using TodoList.Domain.Entities;
using TodoList.Infrastructure.database;

namespace TodoList.Infrastructure.Repositories;

public class TagRepository: ITagRepository
{
    private readonly SqlConnectionFactory _connectionFactory;
    private readonly IMapper _mapper;
    public TagRepository(SqlConnectionFactory connectionFactory, IMapper mapper)
    {
        _connectionFactory = connectionFactory;
        _mapper = mapper;
    }
    
    public async Task<int> CreateAsync(TagCreateDTo tagCreateDTo)
    {
        var tag =  _mapper.Map<Tag>(tagCreateDTo);
        tag.CreatedAt = DateTime.Now;
        tag.UpdatedAt = DateTime.Now;
        
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO tbl_tag(");
        sql.AppendLine("       Name,        ");
        sql.AppendLine("       Color,       ");
        sql.AppendLine("       Description, ");
        sql.AppendLine("       Slug,        ");
        sql.AppendLine("       Status,      ");
        sql.AppendLine("       Created_At,  ");
        sql.AppendLine("       Updated_At   ");
        sql.AppendLine(") VALUES (");
        sql.AppendLine("       @Name,       ");
        sql.AppendLine("       @Color,      ");
        sql.AppendLine("       @Description,");
        sql.AppendLine("       @Slug,       ");
        sql.AppendLine("       @Status,     ");
        sql.AppendLine("       @CreatedAt,  ");
        sql.AppendLine("       @UpdatedAt   ");
        sql.AppendLine(");");
        sql.AppendLine("SELECT LAST_INSERT_ID();");
        var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), tag);
    }

    public async Task<IEnumerable<TagDTo>> GetAllAsync()
    {
        var sql = GetBaseQuery();
        using var connection = _connectionFactory.Create();
        var tags = await connection.QueryAsync<Tag>(sql.ToString());
        return _mapper.Map<IEnumerable<TagDTo>>(tags);
    }

    public async Task<TagDTo?> GetByIdAsync(int id)
    {
        var sql = GetBaseQuery();
        sql.AppendLine("WHERE ID = @Id;");
        using var connection = _connectionFactory.Create();
        var tag = await connection.QueryFirstOrDefaultAsync<Tag>(sql.ToString(), new { Id = id });
        return _mapper.Map<TagDTo>(tag);
    }

    public async Task<int> UpdateAsync(TagUpdateDTo  tagUpdateDTo)
    {
        var tag = _mapper.Map<Tag>(tagUpdateDTo);
        tag.UpdatedAt = DateTime.Now;

        var sql = new StringBuilder();
        sql.AppendLine("UPDATE tbl_tag                    ");
        sql.AppendLine("   SET Name = @Name,              ");
        sql.AppendLine("       Color = @Color,            ");
        sql.AppendLine("       Description = @Description,");
        sql.AppendLine("       Slug = @Slug,              ");
        sql.AppendLine("       Status = @Status,          ");
        sql.AppendLine("       Updated_At = @UpdatedAt    ");
        sql.AppendLine(" WHERE ID = @Id;                  ");

        using var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), tag);
    }

    public  async Task<int> DeleteAsync(int id)
    {
        var sql = new StringBuilder();
        sql.AppendLine("DELETE FROM tbl_tag");
        sql.AppendLine(" WHERE ID = @Id;");
        var connection = _connectionFactory.Create();
        return await connection.ExecuteAsync(sql.ToString(), new { Id = id });
    }
    
    private static StringBuilder GetBaseQuery()
    {
        var sql = new StringBuilder();
        sql.AppendLine("SELECT ID,          ");
        sql.AppendLine("       Name,        ");
        sql.AppendLine("       Color,       ");
        sql.AppendLine("       Description, ");
        sql.AppendLine("       Slug,        ");
        sql.AppendLine("       Status,      ");
        sql.AppendLine("       Created_At,  ");
        sql.AppendLine("       Updated_At   ");
        sql.AppendLine("FROM tbl_tag        ");
        return sql;
    }
}