﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.WeComCore
{
    public interface IWeComCoreSvc
    {
        Task<IWeComResultDto> GetAccessTokenAsync(AccessTokenParam param, string url, CancellationToken cancellationToken = default);

        Task<IWeComResultDto> GetUserIdAsync(GetUserIdParam param, string url, CancellationToken cancellationToken = default);
    }
}
