using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlunosApi.Context;
using AlunosApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AlunosApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlunosController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<Alunos> _logger;

        public AlunosController(DataContext context, ILogger<Alunos> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Alunos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alunos>>> GetAlunos()
        {
          if (_context.Alunos == null)
          {
              _logger.LogCritical("Aluno não existe");
              return NotFound();
          }
            return await _context.Alunos.ToListAsync();
        }

        // GET: api/Alunos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alunos>> GetAlunos(int id)
        {
          if (_context.Alunos == null)
          {
              return NotFound();
          }
            var alunos = await _context.Alunos.FindAsync(id);

            if (alunos == null)
            {
                _logger.LogCritical("Aluno não existe");
                return NotFound();
            }
            
            return alunos;
        }

        // PUT: api/Alunos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlunos(int id, Alunos alunos)
        {
            if (id != alunos.Id)
            {
                _logger.LogCritical("Aluno não existe");
                return BadRequest();
            }

            _context.Entry(alunos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogCritical("Aluno não existe");
                if (!AlunosExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Alunos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Alunos>> PostAlunos(Alunos alunos)
        {
          if (_context.Alunos == null)
          {
              _logger.LogCritical("Aluno não existe");
              return Problem("Entity set 'DataContext.Alunos'  is null.");
          }
            _context.Alunos.Add(alunos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAlunos", new { id = alunos.Id }, alunos);
        }

        // DELETE: api/Alunos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlunos(int id)
        {
            if (_context.Alunos == null)
            {
                _logger.LogCritical("Aluno não existe");
                return NotFound();
            }
            var alunos = await _context.Alunos.FindAsync(id);
            if (alunos == null)
            {
                _logger.LogCritical("Aluno não existe");
                return NotFound();
            }

            _context.Alunos.Remove(alunos);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AlunosExists(int id)
        {
            return (_context.Alunos?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
