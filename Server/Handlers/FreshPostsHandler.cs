using System;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace nboard
{
    class FreshPostsHandlder : IRequestHandler
    {
        private readonly NanoDB _db;

        public FreshPostsHandlder(NanoDB db)
        {
            _db = db;
        }

        public NanoHttpResponse Handle(NanoHttpRequest request)
        {
            try
            {
                return HandleSafe(request);
            }

            catch
            {
                return new ErrorHandler(StatusCode.InternalServerError, "").Handle(request);
            }
        }

        private NanoHttpResponse HandleSafe(NanoHttpRequest request)
        {
            var sb = new StringBuilder();
            ThreadViewHandler.AddHeader(sb);
            var posts = _db.GetNewPosts().ExceptHidden(_db);

            foreach (var p in posts)
            {
                int answers = _db.CountAnswers(p.GetHash());
                string ans = "ответ";
                if (answers != 11 && answers % 10 == 1)
                {
                    //
                }
                else if (answers != 11 && answers % 10 == 5)
                {
                    ans += "ов";
                }
                else
                {
                    ans += "а";
                }

                sb.Append(
                    (
                        p.Message.Replace("\n", "<br/>").ToDiv("postinner", p.GetHash().Value) +
                        (answers > 0 ? ("[" + answers + " " + ans + "]").ToRef("/thread/" + p.GetHash().Value):"") +
                        "[-]".ToButton("", "", @"var x = new XMLHttpRequest(); x.open('POST', '../hide/" + p.GetHash().Value + @"', true);
                        x.send('');
                        document.getElementById('" + p.GetHash().Value + @"').parentNode.style.visibility='hidden';") +
                        //("[В закладки]").ToRef("/bookmark/" + p.GetHash().Value) +
                        ("[В тред]").ToRef("/thread/" + p.ReplyTo.Value) +
                        ("[Ответить]").ToRef("/reply/" + p.GetHash().Value)
                    ).ToDiv("post", ""));
            }

            sb.Append("Обновить".ToButton("", "", "location.reload()").ToDiv("",""));

            return new NanoHttpResponse(StatusCode.Ok, sb.ToString().ToHtmlBody());
        }
    }
    
}