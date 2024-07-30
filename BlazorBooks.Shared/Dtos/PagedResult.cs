using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBooks.Shared.Dtos;

public record PagedResult<TRecord>(TRecord[] Records, int TotalCount);
