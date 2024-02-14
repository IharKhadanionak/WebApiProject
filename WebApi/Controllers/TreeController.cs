using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WebApi.IHandler;
using WebApi.Models.Exceptions;
using WebApi.Models.Nodes;

namespace WebApi.Controllers
{
    [ApiController]
    public class TreeController : ControllerBase
    {
        private readonly IValidator<string> _validator;
        private readonly ITreeHandler _treeHandler;

        public TreeController(IValidator<string> validator, ITreeHandler treeHandler)
        {
            _validator = validator;
            _treeHandler = treeHandler;
        }

        [HttpPost]
        [Route("api.user.tree.get")]
        public async Task<TreeNode> Get(string treeName, CancellationToken token)
        {
            //this is an example that i know about validation issues. better to use here if statement
            var validationResult = _validator.Validate(treeName);
            if (!validationResult.IsValid)
                throw new SecureException($"The skip field is required.");

            var tree = await _treeHandler.GetCreateTreeAsync(treeName, token);
            return tree;
        }
    }
}
