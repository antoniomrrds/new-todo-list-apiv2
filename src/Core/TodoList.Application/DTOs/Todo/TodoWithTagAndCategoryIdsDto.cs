namespace TodoList.Application.DTOs.Todo;

public class TodoWithTagAndCategoryIdsDto: ITagAndCategoryIdsDto 
{
    public int Id { get; set; }
    public string Title { get;  }
    public string Description { get;  }
    public bool IsCompleted { get;  }
    public int Active { get;  }
    public DateTime CreatedAt { get;  }
    public DateTime UpdatedAt { get;  }
    public DateTime? ExpirationDate { get;  }
    public string? ExpirationDateFormatted { get;  }
    public string? CreatedAtFormatted { get;  }
    public string? UpdatedAtFormatted { get;  }
    public List<int>? IdTags { get; set; } 
    public List<int>? IdCategories { get; set; }
    
    private TodoWithTagAndCategoryIdsDto(Builder builder)
    {
        Id = builder.Id;
        Title = builder.Title;
        Description = builder.Description;
        IsCompleted = builder.IsCompleted;
        Active = builder.Active;
        CreatedAt = builder.CreatedAt;
        UpdatedAt = builder.UpdatedAt;
        ExpirationDate = builder.ExpirationDate;
        ExpirationDateFormatted = builder.ExpirationDateFormatted;
        CreatedAtFormatted = builder.CreatedAtFormatted;
        UpdatedAtFormatted = builder.UpdatedAtFormatted;
        IdTags = builder.IdTags;
        IdCategories = builder.IdCategories;
    }

    public class Builder(int id, string title, string description,List<int>? idTags, List<int>? idCategories)
    {
        public int Id { get; } = id;
        public string Title { get; } = title;
        public string Description { get; } = description;
        public bool IsCompleted { get; private set; }
        public int Active { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public string? ExpirationDateFormatted { get; private set; }
        public string? CreatedAtFormatted { get; private set; }
        public string? UpdatedAtFormatted { get; private set; }
        public List<int>? IdTags { get; private set; } = idTags;
        public List<int>? IdCategories { get; private set; } = idCategories;

        public Builder SetIsCompleted(bool isCompleted)
        {
            IsCompleted = isCompleted;
            return this;
        }
        
        public Builder SetActive(int active)
        {
            Active = active;
            return this;
        }
        
        public Builder SetCreatedAt(DateTime createdAt)
        {
            CreatedAt = createdAt;
            return this;
        }
        
        public Builder SetUpdatedAt(DateTime updatedAt)
        {
            UpdatedAt = updatedAt;
            return this;
        }
        
        public Builder SetExpirationDate(DateTime? expirationDate)
        {
            ExpirationDate = expirationDate;
            return this;
        }
        
        public Builder WithFormattedDates(string? expirationDateFormatted, string? createdAtFormatted, string? updatedAtFormatted)
        {
            ExpirationDateFormatted = expirationDateFormatted;
            CreatedAtFormatted = createdAtFormatted;
            UpdatedAtFormatted = updatedAtFormatted;
            return this;
        }
        
        public TodoWithTagAndCategoryIdsDto Build()
        {
            return new TodoWithTagAndCategoryIdsDto(this);
        }
    }
}

public interface ITagAndCategoryIdsDto
{
    public List<int>? IdTags { get; set; }
    public List<int>? IdCategories{ get; set; }
}


public class TagAndCategoryFieldsDTo : ITagAndCategoryIdsDto
{
    public List<int>? IdTags { get; set; }
    public List<int>? IdCategories { get; set; }
}


