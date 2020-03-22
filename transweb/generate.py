
import define as define
from tensor2tensor.utils import registry
from tensor2tensor import problems

t2t_problem = problems.problem(define.PROBLEM)
t2t_problem.generate_data(define.DATA_DIR, define.TMP_DIR) 