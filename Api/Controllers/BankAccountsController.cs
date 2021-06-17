using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Api.Enities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Cors;

namespace Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors]

    public class BankAccountsController : ControllerBase
    {
        private readonly FreeLancerVNContext _context;

        public BankAccountsController(FreeLancerVNContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            return await _context.BankAccounts.Include(p=>p.Bank).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts.Include(p=>p.Bank)
                .SingleOrDefaultAsync(p=>p.Id==id);
            
            if (bankAccount == null)
            {
                return NotFound();
            }

            return bankAccount;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccountPostModel 
            bankAccountPostModel)
        {
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            Account account = _context.Accounts.Include(p=>p.BankAccounts)
                .SingleOrDefault(p => p.Email == email);
            if (account == null)
            {
                return BadRequest();
            }
            BankAccount bankAccount = account.BankAccounts.SingleOrDefault(p => p.Id == id);
            bankAccount.BankId = bankAccountPostModel.BankId;
            bankAccount.OwnerName = bankAccountPostModel.OwnerName;
            bankAccount.AccountNumber = bankAccountPostModel.AccountNumber;
            bankAccount.BranchName = bankAccountPostModel.BranchName;
            _context.Entry(bankAccount).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccountPostModel
            bankAccountPostModel)
        {
            String jwt = Request.Headers["Authorization"];
            jwt = jwt.Substring(7);
            //Decode jwt and get payload
            var stream = jwt;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            var tokenS = jsonToken as JwtSecurityToken;
            //I can get Claims using:
            var email = tokenS.Claims.First(claim => claim.Type == "email").Value;
            Account account = _context.Accounts.SingleOrDefault(p => p.Email == email);
            if (account == null)
            {
                return BadRequest();
            }

            BankAccount bankAccount = new BankAccount()
            {
                BankId = bankAccountPostModel.BankId,
                AccountId = account.Id,
                OwnerName = bankAccountPostModel.OwnerName,
                AccountNumber = bankAccountPostModel.AccountNumber,
                BranchName = bankAccountPostModel.BranchName,
            };
            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BankAccount>> DeleteBankAccount(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return bankAccount;
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.Id == id);
        }
    }
}
