namespace DevJobs.API.Controllers
{
    using DevJobs.API.Entities;
    using DevJobs.API.Models;
    using DevJobs.API.Persistence;
    using DevJobs.API.Persistence.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Serilog;

    [Route("api/job-vacancies")]
    [ApiController]
    public class JobVacanciesController : ControllerBase
    {
        private readonly IJobVacancyRepository _repository;
        // public JobVacanciesController(DevJobsContext context) usando contexto
        public JobVacanciesController(IJobVacancyRepository repository) // usando padrão Repository
        {
            _repository = repository;
        }

        // GET api/job-vacancies
        [HttpGet]
        public IActionResult GetAll()
        {
            var jobVacancies = _repository.GetAll();
            
            return Ok(jobVacancies);
        }

        // GET api/job-vacancies/4
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var jobVacancy = _repository.GetById(id);

            if (jobVacancy == null)
                return NotFound();
            
            return Ok(jobVacancy);
        }

        // POST api/job-vacancies
        /// <summary>
        /// Cadastrar uma vaga de emprego.
        /// </summary>
        /// <remarks>
        /// {
        ///    "title": "Dev .NET Core Jr",
        ///    "description": "Nível Junior",
        ///    "company": "ASD iO",
        ///    "isRemote": true,
        ///    "salaryRange": "3000"
        /// }
        /// </remarks>
        /// <param name="model">Dados da vaga.</param>
        /// <returns>Objeto recém criado.</returns>
        /// <response code="201">Sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public IActionResult Post(AddJobVacancyInputModel model)
        {
            Log.Information("POST JobVacancy chamado");

            var jobVacancy = new JobVacancy(
                model.Title,
                model.Description,
                model.Company,
                model.IsRemote,
                model.SalaryRange
            );

            if (jobVacancy.Title.Length > 30)
            {
                return BadRequest("O título precisa ter menos de 30 caracteres.");
            }

            _repository.Add(jobVacancy);

            return CreatedAtAction(
                "GetById",
                new { id = jobVacancy.Id },
                jobVacancy);
        }

        // PUT api/job-vacancies/4
        [HttpPut("{id}")]
        public IActionResult Put(int id, UpdateJobVacancyInputModel model)
        {
            var jobVacancy = _repository.GetById(id);
            
            if (jobVacancy == null)
                return NotFound();
            
            jobVacancy.Update(model.Title, model.Description);
            
            _repository.Update(jobVacancy);
            
            return NoContent();
        }
    }
}